INSERT INTO zona (nombre, descripcion)
VALUES
('Zona Industrial', 'Clientes ubicados en la zona industrial'),
('Uchumayo', 'Clientes ubicados por Uchumayo y alrededores'),
('Cercado', 'Clientes cercanos al centro de Arequipa');

INSERT INTO cliente
(razon_social, ruc, telefono, direccion, id_zona, tipo_cliente, requiere_garantia)
VALUES
('Cliente Demo Industrial SAC', '20123456789', '999111222',
 'Av. Industrial 123', 1, 'FRECUENTE', FALSE),
('Taller Demo EIRL', '20987654321', '988777666',
 'Uchumayo km 5', 2, 'EVENTUAL', TRUE),
('Paciente Particular Demo', NULL, '955444333',
 'Cercado', 3, 'EVENTUAL', TRUE);

INSERT INTO producto
(codigo, nombre, tipo_producto, unidad_medida, precio_referencia)
VALUES
('OX-M3', 'Oxígeno Industrial', 'GAS', 'M3', 14.30),
('CO2-KG', 'Dióxido de Carbono', 'GAS', 'KG', 8.50),
('ACE-KG', 'Acetileno', 'GAS', 'KG', 25.00),
('EQ-SOLD-001', 'Máquina de soldar inverter', 'EQUIPO', 'UND', 850.00),
('INS-ELEC-6011', 'Electrodo 6011 3.25 mm', 'INSUMO', 'KG', 12.50);

INSERT INTO cilindro
(codigo_cilindro, id_producto, capacidad, propietario_tipo,
 id_cliente_propietario, estado_actual, ubicacion_actual)
VALUES
('OX-DEMO-001', 1, 10, 'EMPRESA', NULL, 'LLENO_ALMACEN', 'Almacén principal'),
('OX-DEMO-002', 1, 6, 'EMPRESA', NULL, 'VACIO_ALMACEN', 'Zona de vacíos'),
('CO2-DEMO-001', 2, 25, 'CLIENTE', 2, 'EN_CLIENTE', 'Taller Demo EIRL');

INSERT INTO conductor (nombre, telefono)
VALUES
('Conductor Demo 1', '999111111'),
('Conductor Demo 2', '999222222');

INSERT INTO vehiculo (placa, descripcion)
VALUES
('DEM-001', 'Vehículo de reparto zona industrial'),
('DEM-002', 'Vehículo de reparto Uchumayo');

INSERT INTO pedido
(id_cliente, direccion_entrega, id_zona, estado_pedido, observaciones)
VALUES
(
    2,
    'Uchumayo km 5',
    2,
    'PENDIENTE',
    'Cliente solicita entrega durante la mañana'
);

INSERT INTO detalle_pedido
(id_pedido, id_producto, cantidad, precio_unitario, subtotal)
VALUES
(1, 1, 20, 14.30, 286.00),
(1, 2, 10, 8.50, 85.00);

INSERT INTO movimiento_cilindro
(id_cilindro, id_pedido, tipo_movimiento, id_cliente, id_conductor, id_vehiculo, observacion)
VALUES
(
    1,
    1,
    'SALIDA_CLIENTE',
    2,
    1,
    1,
    'Cilindro lleno entregado al cliente'
);

INSERT INTO proveedor
(razon_social, ruc, telefono, direccion)
VALUES
('Proveedor Demo Gases SAC', '20111222333', '999888777', 'Arequipa'),
('Proveedor Demo Industrial SAC', '20444555666', '988777666', 'Arequipa');

INSERT INTO envio_recarga
(id_proveedor, numero_guia, estado, observaciones)
VALUES
(
    1,
    'GUIA-DEMO-001',
    'ENVIADO',
    'Envío de cilindros vacíos para recarga'
);

INSERT INTO detalle_envio_recarga
(id_envio, id_cilindro)
VALUES
(1, 2);

INSERT INTO movimiento_cilindro
(id_cilindro, tipo_movimiento, observacion)
VALUES
(
    2,
    'ENVIO_RECARGA',
    'Cilindro enviado al proveedor para recarga - GUIA-DEMO-001'
);
