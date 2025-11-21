import json
import os
from typing import List
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

app = FastAPI()

DB_FILE = "datos.json"

db = {
    "productos": [],
    "ventas": []
}

# Variables globales para IDs
next_producto_id = 1
next_venta_id = 1


class Producto(BaseModel):
    id: int = None
    NOMBRE: str
    CATEGORIA: str
    PRECIOKG: float
    STOCK: float


class Venta(BaseModel):
    id: int = None
    producto_id: int
    kilos_vendidos: float
    total_venta: float = None


def guardar_datos():
    data_to_save = {
        "productos": [p.dict() for p in db["productos"]],
        "ventas": [v.dict() for v in db["ventas"]],
        "next_producto_id": next_producto_id,
        "next_venta_id": next_venta_id
    }
    with open(DB_FILE, "w") as f:
        json.dump(data_to_save, f, indent=4)


def cargar_datos():
    global next_producto_id, next_venta_id
    if not os.path.exists(DB_FILE):
        db["productos"] = [
            Producto(id=1, NOMBRE="Manzana Fuji", CATEGORIA="Fruta", PRECIOKG=2.50, STOCK=100.0),
            Producto(id=2, NOMBRE="Pera Conferencia", CATEGORIA="Fruta", PRECIOKG=1.80, STOCK=50.0)
        ]
        global next_producto_id
        next_producto_id = 3
        guardar_datos()
        return

    with open(DB_FILE, "r") as f:
        data = json.load(f)
        db["productos"] = [Producto(**p) for p in data["productos"]]
        db["ventas"] = [Venta(**v) for v in data["ventas"]]
        next_producto_id = data.get("next_producto_id", 1)
        next_venta_id = data.get("next_venta_id", 1)


cargar_datos()


@app.get("/productos", response_model=List[Producto])
def listar_productos():
    return db["productos"]


@app.post("/productos", response_model=Producto)
def crear_producto(producto: Producto):
    global next_producto_id
    producto.id = next_producto_id
    next_producto_id += 1
    db["productos"].append(producto)
    guardar_datos()
    return producto


@app.put("/productos/{id}", response_model=Producto)
def actualizar_producto(id: int, producto_actualizado: Producto):
    for i, p in enumerate(db["productos"]):
        if p.id == id:
            producto_actualizado.id = id
            db["productos"][i] = producto_actualizado
            guardar_datos()
            return producto_actualizado
    raise HTTPException(status_code=404, detail="Producto no encontrado")


@app.delete("/productos/{id}")
def eliminar_producto(id: int):
    global db
    antes = len(db["productos"])
    db["productos"] = [p for p in db["productos"] if p.id != id]
    if len(db["productos"]) == antes:
        raise HTTPException(status_code=404, detail="Producto no encontrado")
    guardar_datos()
    return {"ok": True}


@app.post("/ventas", response_model=Venta)
def registrar_venta(venta_data: Venta):
    global next_venta_id

    # Buscar producto
    producto = next((p for p in db["productos"] if p.id == venta_data.producto_id), None)
    if not producto:
        raise HTTPException(status_code=404, detail="Producto no encontrado")

    if producto.STOCK < venta_data.kilos_vendidos:
        raise HTTPException(status_code=400, detail="Stock insuficiente")

    # Crear venta
    total = venta_data.kilos_vendidos * producto.PRECIOKG
    nueva_venta = Venta(
        id=next_venta_id,
        producto_id=venta_data.producto_id,
        kilos_vendidos=venta_data.kilos_vendidos,
        total_venta=round(total, 2)
    )
    next_venta_id += 1

    # Actualizar stock
    producto.STOCK = round(producto.STOCK - venta_data.kilos_vendidos, 2)

    db["ventas"].append(nueva_venta)
    guardar_datos()
    return nueva_venta


@app.get("/ventas", response_model=List[Venta])
def listar_ventas():
    return db["ventas"]