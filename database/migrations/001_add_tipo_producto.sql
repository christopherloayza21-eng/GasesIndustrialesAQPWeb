ALTER TABLE producto
ADD COLUMN IF NOT EXISTS tipo_producto VARCHAR(20) NOT NULL DEFAULT 'GAS';

ALTER TABLE producto
DROP CONSTRAINT IF EXISTS chk_producto_tipo;

ALTER TABLE producto
ADD CONSTRAINT chk_producto_tipo
CHECK (tipo_producto IN ('GAS', 'EQUIPO', 'INSUMO', 'SERVICIO'));
