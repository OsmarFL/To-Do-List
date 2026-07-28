// URL base de la API — ajusta el puerto según tu proyecto
const API_URL = "/api/Tarea";

// Variable para recordar el filtro activo
let filtroActual = "todos";

// =============================
// UTILIDADES DE UI
// =============================

// Muestra u oculta el loader
function setLoader(visible) {
    const loader = document.getElementById("loader");
    if (visible) {
        loader.classList.remove("oculto");
    } else {
        loader.classList.add("oculto");
    }
}

// Muestra un mensaje de error en pantalla
function setError(mensaje) {
    const el = document.getElementById("mensajeError");
    if (mensaje) {
        el.textContent = "⚠️ " + mensaje;
        el.classList.remove("oculto");
    } else {
        el.classList.add("oculto");
    }
}

// Activa visualmente el botón de filtro seleccionado
function setFiltroActivo(filtro) {
    document.querySelectorAll(".btn-filtro").forEach(btn => {
        btn.classList.remove("activo");
    });

    const mapa = { "todos": 0, "Pendiente": 1, "Completada": 2 };
    const botones = document.querySelectorAll(".btn-filtro");
    if (botones[mapa[filtro]]) {
        botones[mapa[filtro]].classList.add("activo");
    }
}

// =============================
// CARGAR Y MOSTRAR TAREAS
// =============================

// Obtiene las tareas desde la API y las renderiza.
// Usa fetch() con Promesas encadenadas (.then / .catch)
function cargarTareas(filtro) {
    filtroActual = filtro || "todos";
    setFiltroActivo(filtroActual);
    setLoader(true);
    setError(null);

    // Construye la URL con el filtro de estado si aplica
    const url = filtroActual !== "todos"
        ? `${API_URL}?status=${filtroActual}`
        : API_URL;

    // fetch() retorna una Promesa
    fetch(url)
        .then(function (response) {
            // Si la respuesta no es exitosa, lanza un error
            if (!response.ok) {
                throw new Error("Error al obtener las tareas: " + response.status);
            }
            // .json() también retorna una Promesa
            return response.json();
        })
        .then(function (tareas) {
            // Una vez resuelta, renderiza las tarjetas
            renderizarTareas(tareas);
            setLoader(false);
        })
        .catch(function (error) {
            // .catch() captura cualquier error de la cadena
            setError(error.message);
            setLoader(false);
        });
}

// Crea dinámicamente las tarjetas de tareas en el DOM
function renderizarTareas(tareas) {
    const contenedor = document.getElementById("contenedorTareas");

    // Limpia el contenedor antes de insertar
    contenedor.innerHTML = "";

    if (tareas.length === 0) {
        contenedor.innerHTML = "<p style='color:#94a3b8;'>No hay tareas para mostrar.</p>";
        return;
    }

    // Crea un elemento HTML por cada tarea usando manipulación del DOM
    tareas.forEach(function (tarea) {
        const card = document.createElement("div");
        card.className = "tarea-card" + (tarea.estado === "Completada" ? " completada" : "");

        // Badge de estado
        const badgeClase = tarea.estado === "Completada" ? "badge-completada" : "badge-pendiente";

        // Fecha límite formateada
        const fechaLimite = tarea.fechaLimite
            ? new Date(tarea.fechaLimite).toLocaleDateString("es-DO")
            : "Sin fecha límite";

        const fechaCreacion = new Date(tarea.fechaCreacion).toLocaleDateString("es-DO");

        card.innerHTML = `
      <span class="badge ${badgeClase}">${tarea.estado}</span>
      <p class="tarea-titulo">${tarea.titulo}</p>
      <p class="tarea-descripcion">${tarea.descripcion || "Sin descripción"}</p>
      <p class="tarea-info">📅 Creada: ${fechaCreacion}</p>
      <p class="tarea-info">⏰ Límite: ${fechaLimite}</p>
      <div class="tarea-acciones">
        ${tarea.estado === "Pendiente"
                ? `<button class="btn btn-completar" onclick="completarTarea(${tarea.id})">Completar</button>`
                : ""}
        <button class="btn btn-eliminar" onclick="eliminarTarea(${tarea.id})">Eliminar</button>
      </div>
    `;

        // Agrega la tarjeta al contenedor usando manipulación del DOM
        contenedor.appendChild(card);
    });
}

// =============================
// CREAR TAREA
// =============================

// Usa async/await (otra forma de manejar Promesas)
async function crearTarea() {
    const titulo = document.getElementById("titulo").value.trim();
    const descripcion = document.getElementById("descripcion").value.trim();
    const fechaLimite = document.getElementById("fechaLimite").value;

    // Validación en el frontend antes de llamar la API
    if (!titulo) {
        setError("El título es obligatorio.");
        return;
    }

    const nuevaTarea = {
        titulo: titulo,
        descripcion: descripcion,
        fechaLimite: fechaLimite ? new Date(fechaLimite).toISOString() : null
    };

    // try/catch para manejar errores con async/await
    try {
        setLoader(true);
        setError(null);

        const response = await fetch(API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(nuevaTarea)
        });

        if (!response.ok) {
            throw new Error("Error al crear la tarea: " + response.status);
        }

        // Limpia el formulario
        document.getElementById("titulo").value = "";
        document.getElementById("descripcion").value = "";
        document.getElementById("fechaLimite").value = "";

        // Recarga las tareas para ver la nueva
        cargarTareas(filtroActual);

    } catch (error) {
        setError(error.message);
        setLoader(false);
    }
}

// =============================
// COMPLETAR TAREA
// =============================

function completarTarea(id) {
    fetch(`${API_URL}/${id}/complete`, { method: "PATCH" })
        .then(function (response) {
            if (!response.ok) {
                throw new Error("No se pudo completar la tarea.");
            }
            return response.json();
        })
        .then(function () {
            cargarTareas(filtroActual);
        })
        .catch(function (error) {
            setError(error.message);
        });
}

// =============================
// ELIMINAR TAREA
// =============================

function eliminarTarea(id) {
    if (!confirm("¿Seguro que deseas eliminar esta tarea?")) return;

    fetch(`${API_URL}/${id}`, { method: "DELETE" })
        .then(function (response) {
            if (!response.ok) {
                throw new Error("No se pudo eliminar la tarea.");
            }
            cargarTareas(filtroActual);
        })
        .catch(function (error) {
            setError(error.message);
        });
}

// =============================
// FILTRAR TAREAS
// =============================

function filtrar(estado) {
    cargarTareas(estado);
}

// =============================================
// INICIO — carga las tareas al abrir la página
// =============================================
cargarTareas("todos");