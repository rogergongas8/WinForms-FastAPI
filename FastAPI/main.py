from typing import List

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

# Inicializamos la aplicacion de FastApi
app = FastAPI()

# Simulacion de almacenamiento de datos
db = {
    "productos": [],
    "ventas": []
}

next_producto_id = 1
next_venta_id = 1

class Producto(BaseModel):
    id: int = None
    nombre: str
    categoria: str
    preciokg: float
    stockkg: float

class Venta(BaseModel):
    id: int = None
    producto_id: int # Corregido: Usamos producto_id para claridad
    kilos_vendidos: float
    total_venta: float = None


db["productos"].extend([
    Producto(id=1, nombre="Manzana Fuji", categoria="Fruta", preciokg=2.50, stockkg=100.0),
    Producto(id=2, nombre="Pera Conferencia", categoria="Fruta", preciokg=1.80, stockkg=50.0)
])
next_producto_id = 3

# Metodo GET para obtener los productos
@app.get("/productos", response_model=List[Producto])
def listar_productos():
    return db["productos"]

# Metodo post para crear los productos
@app.post("/productos", response_model=Producto)
def crear_producto(producto: Producto):
    global next_producto_id

    producto.id = next_producto_id
    next_producto_id += 1

    db["productos"].append(producto)
    return producto


# Metodo put para actualizar un producto mediante su id
@app.put("/productos/{id}", response_model=Producto)
def actualizar_producto(id: int, producto_actualizado: Producto):
    for i, producto_actual in enumerate(db["productos"]):
        if producto_actual.id == id:
            producto_actualizado.id = id
            db["productos"][i] = producto_actualizado
            return producto_actualizado

    raise HTTPException(status_code=404, detail=f"Producto con ID {id} no encontrado")


# Metodo DELETE para borrar un producto mediante su id
@app.delete("/productos/{id}")
def eliminar_producto(id: int):
    global db
    productos_antes = len(db["productos"])

    db["productos"] = [p for p in db["productos"] if p.id != id]

    if len(db["productos"]) == productos_antes:
        raise HTTPException(status_code=404, detail=f"Producto con ID {id} no encontrado")

    return {"ok": True}



# Metodo POST para una venta
@app.post("/ventas", response_model=Venta)
def registrar_venta(venta_data: Venta):
    global next_venta_id

    # 1. Búsqueda del producto por ID
    producto_index = -1
    producto_encontrado = None
    for i, p in enumerate(db["productos"]):
        if p.id == venta_data.producto_id:
            producto_encontrado = p
            producto_index = i
            break

    if producto_index == -1:
        raise HTTPException(status_code=404, detail=f"Producto con ID {venta_data.producto_id} no encontrado")

    # 2. Verificar las condiciones y el stock
    if venta_data.kilos_vendidos <= 0:
        raise HTTPException(status_code=400, detail="Los kilos vendidos deben ser mayores a cero.")

    if producto_encontrado.stockkg < venta_data.kilos_vendidos:
        raise HTTPException(status_code=400,
                            detail=f"Stock insuficiente. Disponible: {producto_encontrado.stockkg} kg.")

    # 3. Calcular total
    total = venta_data.kilos_vendidos * producto_encontrado.preciokg

    # 4. Crear el objeto Venta completo
    nueva_venta = Venta(
        id=next_venta_id,
        producto_id=venta_data.producto_id,
        kilos_vendidos=venta_data.kilos_vendidos,
        total_venta=round(total, 2)
    )
    next_venta_id += 1

    producto_encontrado.stockkg = round(producto_encontrado.stockkg - venta_data.kilos_vendidos, 2)

    # 6. Guardar la venta
    db["ventas"].append(nueva_venta)

    return nueva_venta()

@app.get("/ventas", response_model=List[Venta])
def listar_ventas():
    return db["ventas"]