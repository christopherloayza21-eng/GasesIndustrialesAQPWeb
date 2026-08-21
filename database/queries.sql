SELECT version();

SELECT * FROM zona;
SELECT * FROM cliente;
SELECT * FROM producto;
SELECT * FROM cilindro;
SELECT * FROM conductor;
SELECT * FROM vehiculo;
SELECT * FROM pedido;
SELECT * FROM detalle_pedido;
SELECT * FROM movimiento_cilindro;
SELECT * FROM proveedor;
SELECT * FROM envio_recarga;
SELECT * FROM detalle_envio_recarga;

SELECT
    c.id_cliente,
    c.razon_social,
    c.ruc,
    z.nombre AS zona,
    c.tipo_cliente,
    c.requiere_garantia
FROM cliente c
LEFT JOIN zona z
    ON c.id_zona = z.id_zona;

SELECT
    ci.id_cilindro,
    ci.codigo_cilindro,
    p.nombre AS producto,
    ci.capacidad,
    ci.propietario_tipo,
    c.razon_social AS propietario_cliente,
    ci.estado_actual,
    ci.ubicacion_actual
FROM cilindro ci
JOIN producto p
    ON ci.id_producto = p.id_producto
LEFT JOIN cliente c
    ON ci.id_cliente_propietario = c.id_cliente;

SELECT
    pe.id_pedido,
    pe.fecha_pedido,
    c.razon_social AS cliente,
    p.codigo,
    p.nombre AS producto,
    dp.cantidad,
    p.unidad_medida,
    dp.precio_unitario,
    dp.subtotal,
    pe.estado_pedido
FROM pedido pe
JOIN cliente c
    ON pe.id_cliente = c.id_cliente
JOIN detalle_pedido dp
    ON pe.id_pedido = dp.id_pedido
JOIN producto p
    ON dp.id_producto = p.id_producto
ORDER BY pe.id_pedido;

SELECT
    mc.id_movimiento,
    ci.codigo_cilindro,
    p.nombre AS producto,
    mc.tipo_movimiento,
    mc.fecha_movimiento,
    c.razon_social AS cliente,
    co.nombre AS conductor,
    v.placa,
    mc.observacion
FROM movimiento_cilindro mc
JOIN cilindro ci
    ON mc.id_cilindro = ci.id_cilindro
JOIN producto p
    ON ci.id_producto = p.id_producto
LEFT JOIN cliente c
    ON mc.id_cliente = c.id_cliente
LEFT JOIN conductor co
    ON mc.id_conductor = co.id_conductor
LEFT JOIN vehiculo v
    ON mc.id_vehiculo = v.id_vehiculo;

UPDATE cilindro
SET
    estado_actual = 'EN_CLIENTE',
    ubicacion_actual = 'Taller Demo EIRL',
    fecha_ultimo_movimiento = CURRENT_TIMESTAMP
WHERE id_cilindro = 1;

SELECT
    codigo_cilindro,
    estado_actual,
    ubicacion_actual,
    fecha_ultimo_movimiento
FROM cilindro
WHERE id_cilindro = 1;

SELECT
    id_cilindro,
    codigo_cilindro,
    estado_actual
FROM cilindro;

UPDATE cilindro
SET
    estado_actual = 'EN_PROVEEDOR',
    ubicacion_actual = 'Proveedor Demo Gases SAC',
    fecha_ultimo_movimiento = CURRENT_TIMESTAMP
WHERE id_cilindro = 2;

SELECT
    codigo_cilindro,
    estado_actual,
    ubicacion_actual
FROM cilindro
WHERE id_cilindro = 2;

UPDATE detalle_envio_recarga
SET
    fecha_retorno = CURRENT_TIMESTAMP,
    estado_retorno = 'RECIBIDO',
    observacion = 'Cilindro recibido lleno desde proveedor'
WHERE id_envio = 1
  AND id_cilindro = 2;

INSERT INTO movimiento_cilindro
(id_cilindro, tipo_movimiento, observacion)
VALUES
(
    2,
    'RETORNO_RECARGA',
    'Cilindro retornó lleno desde proveedor - GUIA-DEMO-001'
);

UPDATE cilindro
SET
    estado_actual = 'LLENO_ALMACEN',
    ubicacion_actual = 'Almacén principal',
    fecha_ultimo_movimiento = CURRENT_TIMESTAMP
WHERE id_cilindro = 2;

UPDATE envio_recarga
SET estado = 'COMPLETADO'
WHERE id_envio = 1;

SELECT
    ci.codigo_cilindro,
    ci.estado_actual,
    ci.ubicacion_actual,
    der.estado_retorno,
    der.fecha_retorno,
    er.estado AS estado_envio
FROM detalle_envio_recarga der
JOIN cilindro ci
    ON der.id_cilindro = ci.id_cilindro
JOIN envio_recarga er
    ON der.id_envio = er.id_envio
WHERE der.id_envio = 1;
