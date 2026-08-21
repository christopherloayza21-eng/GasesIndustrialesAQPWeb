CREATE TABLE zona (
    id_zona SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL UNIQUE,
    descripcion TEXT,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE cliente (
    id_cliente SERIAL PRIMARY KEY,
    razon_social VARCHAR(150) NOT NULL,
    ruc VARCHAR(11) UNIQUE,
    telefono VARCHAR(20),
    direccion TEXT,
    id_zona INT,
    tipo_cliente VARCHAR(20) NOT NULL DEFAULT 'EVENTUAL',
    requiere_garantia BOOLEAN NOT NULL DEFAULT TRUE,
    activo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_cliente_zona
        FOREIGN KEY (id_zona)
        REFERENCES zona(id_zona),

    CONSTRAINT chk_cliente_tipo
        CHECK (tipo_cliente IN ('NUEVO', 'FRECUENTE', 'EVENTUAL'))
);

CREATE TABLE producto (
    id_producto SERIAL PRIMARY KEY,
    codigo VARCHAR(20) NOT NULL UNIQUE,
    nombre VARCHAR(100) NOT NULL,
    tipo_producto VARCHAR(20) NOT NULL DEFAULT 'GAS',
    unidad_medida VARCHAR(10) NOT NULL,
    precio_referencia NUMERIC(10,2),
    activo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT chk_producto_tipo
        CHECK (tipo_producto IN ('GAS', 'EQUIPO', 'INSUMO', 'SERVICIO')),

    CONSTRAINT chk_producto_precio
        CHECK (precio_referencia IS NULL OR precio_referencia >= 0)
);

CREATE TABLE cilindro (
    id_cilindro SERIAL PRIMARY KEY,
    codigo_cilindro VARCHAR(50) NOT NULL UNIQUE,
    id_producto INT NOT NULL,
    capacidad NUMERIC(10,2),
    propietario_tipo VARCHAR(20) NOT NULL,
    id_cliente_propietario INT,
    estado_actual VARCHAR(30) NOT NULL,
    ubicacion_actual VARCHAR(100),
    fecha_ultimo_movimiento TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_cilindro_producto
        FOREIGN KEY (id_producto)
        REFERENCES producto(id_producto),

    CONSTRAINT fk_cilindro_cliente_propietario
        FOREIGN KEY (id_cliente_propietario)
        REFERENCES cliente(id_cliente),

    CONSTRAINT chk_cilindro_propietario
        CHECK (propietario_tipo IN ('EMPRESA', 'CLIENTE')),

    CONSTRAINT chk_cilindro_estado
        CHECK (
            estado_actual IN (
                'LLENO_ALMACEN',
                'VACIO_ALMACEN',
                'EN_REPARTO',
                'EN_CLIENTE',
                'EN_RECARGA',
                'EN_PROVEEDOR',
                'BAJA'
            )
        ),

    CONSTRAINT chk_cilindro_cliente_propietario
        CHECK (
            (propietario_tipo = 'EMPRESA' AND id_cliente_propietario IS NULL)
            OR
            (propietario_tipo = 'CLIENTE' AND id_cliente_propietario IS NOT NULL)
        )
);

CREATE TABLE conductor (
    id_conductor SERIAL PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    telefono VARCHAR(20),
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE vehiculo (
    id_vehiculo SERIAL PRIMARY KEY,
    placa VARCHAR(10) NOT NULL UNIQUE,
    descripcion VARCHAR(150),
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE pedido (
    id_pedido SERIAL PRIMARY KEY,
    id_cliente INT NOT NULL,
    fecha_pedido TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    direccion_entrega TEXT,
    id_zona INT,
    id_conductor INT,
    id_vehiculo INT,
    estado_pedido VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE',
    observaciones TEXT,

    CONSTRAINT fk_pedido_cliente
        FOREIGN KEY (id_cliente)
        REFERENCES cliente(id_cliente),

    CONSTRAINT fk_pedido_zona
        FOREIGN KEY (id_zona)
        REFERENCES zona(id_zona),

    CONSTRAINT fk_pedido_conductor
        FOREIGN KEY (id_conductor)
        REFERENCES conductor(id_conductor),

    CONSTRAINT fk_pedido_vehiculo
        FOREIGN KEY (id_vehiculo)
        REFERENCES vehiculo(id_vehiculo),

    CONSTRAINT chk_pedido_estado
        CHECK (
            estado_pedido IN (
                'PENDIENTE',
                'ASIGNADO',
                'EN_REPARTO',
                'ENTREGADO',
                'CANCELADO'
            )
        )
);

CREATE TABLE detalle_pedido (
    id_detalle SERIAL PRIMARY KEY,
    id_pedido INT NOT NULL,
    id_producto INT NOT NULL,
    cantidad NUMERIC(10,2) NOT NULL,
    precio_unitario NUMERIC(10,2),
    subtotal NUMERIC(12,2),

    CONSTRAINT fk_detalle_pedido
        FOREIGN KEY (id_pedido)
        REFERENCES pedido(id_pedido),

    CONSTRAINT fk_detalle_producto
        FOREIGN KEY (id_producto)
        REFERENCES producto(id_producto),

    CONSTRAINT chk_detalle_cantidad
        CHECK (cantidad > 0),

    CONSTRAINT chk_detalle_precio
        CHECK (precio_unitario IS NULL OR precio_unitario >= 0),

    CONSTRAINT chk_detalle_subtotal
        CHECK (subtotal IS NULL OR subtotal >= 0)
);

CREATE TABLE movimiento_cilindro (
    id_movimiento SERIAL PRIMARY KEY,
    id_cilindro INT NOT NULL,
    id_pedido INT,
    tipo_movimiento VARCHAR(30) NOT NULL,
    fecha_movimiento TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    id_cliente INT,
    id_conductor INT,
    id_vehiculo INT,
    observacion TEXT,

    CONSTRAINT fk_movimiento_cilindro
        FOREIGN KEY (id_cilindro)
        REFERENCES cilindro(id_cilindro),

    CONSTRAINT fk_movimiento_pedido
        FOREIGN KEY (id_pedido)
        REFERENCES pedido(id_pedido),

    CONSTRAINT fk_movimiento_cliente
        FOREIGN KEY (id_cliente)
        REFERENCES cliente(id_cliente),

    CONSTRAINT fk_movimiento_conductor
        FOREIGN KEY (id_conductor)
        REFERENCES conductor(id_conductor),

    CONSTRAINT fk_movimiento_vehiculo
        FOREIGN KEY (id_vehiculo)
        REFERENCES vehiculo(id_vehiculo)
);

CREATE TABLE proveedor (
    id_proveedor SERIAL PRIMARY KEY,
    razon_social VARCHAR(150) NOT NULL,
    ruc VARCHAR(11) UNIQUE,
    telefono VARCHAR(20),
    direccion TEXT,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE envio_recarga (
    id_envio SERIAL PRIMARY KEY,
    id_proveedor INT NOT NULL,
    fecha_envio TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    numero_guia VARCHAR(50),
    estado VARCHAR(20) NOT NULL DEFAULT 'ENVIADO',
    observaciones TEXT,

    CONSTRAINT fk_envio_proveedor
        FOREIGN KEY (id_proveedor)
        REFERENCES proveedor(id_proveedor),

    CONSTRAINT chk_envio_estado
        CHECK (estado IN ('PREPARADO', 'ENVIADO', 'PARCIAL', 'COMPLETADO', 'CANCELADO'))
);

CREATE TABLE detalle_envio_recarga (
    id_detalle_envio SERIAL PRIMARY KEY,
    id_envio INT NOT NULL,
    id_cilindro INT NOT NULL,
    fecha_retorno TIMESTAMP,
    estado_retorno VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE',
    observacion TEXT,

    CONSTRAINT fk_detalle_envio
        FOREIGN KEY (id_envio)
        REFERENCES envio_recarga(id_envio),

    CONSTRAINT fk_detalle_envio_cilindro
        FOREIGN KEY (id_cilindro)
        REFERENCES cilindro(id_cilindro),

    CONSTRAINT uq_envio_cilindro
        UNIQUE (id_envio, id_cilindro),

    CONSTRAINT chk_estado_retorno
        CHECK (estado_retorno IN ('PENDIENTE', 'RECIBIDO', 'OBSERVADO'))
);
