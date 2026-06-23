import { useEffect, useState } from "react";

export default function AdminServices() {
  const [services, setServices] = useState([]);
  const [loading, setLoading] = useState(true);

  const [newService, setNewService] = useState({
    name: "",
    durationMinutes: "",
    price: "",
  });

  const [editingService, setEditingService] = useState(null);

  const token = localStorage.getItem("token");

  const loadServices = async () => {
    try {
      setLoading(true);

      const res = await fetch("http://localhost:5078/api/services");
      const data = await res.json();

      setServices(data);
    } catch (err) {
      console.error("Failed to load services", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadServices();
  }, []);

  const createService = async () => {
    try {
      await fetch("http://localhost:5078/api/services", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(newService),
      });

      setNewService({ name: "", durationMinutes: "", price: "" });
      loadServices();
    } catch (err) {
      console.error("Failed to create service", err);
    }
  };

  const updateService = async () => {
    try {
      await fetch(`http://localhost:5078/api/services/${editingService.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(editingService),
      });

      setEditingService(null);
      loadServices();
    } catch (err) {
      console.error("Failed to update service", err);
    }
  };

  const deleteService = async (id) => {
    if (!confirm("Are you sure you want to delete this service?")) return;

    try {
      await fetch(`http://localhost:5078/api/services/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      loadServices();
    } catch (err) {
      console.error("Failed to delete service", err);
    }
  };

  return (
    <div className="space-y-10">

      {/* Add Service */}
      <div className="p-6 bg-white border rounded-lg shadow-sm">
        <h2 className="text-xl font-semibold mb-4">Add New Service</h2>

        <div className="grid md:grid-cols-3 gap-4">
          <input
            placeholder="Service Name"
            className="border p-2 rounded"
            value={newService.name}
            onChange={(e) =>
              setNewService({ ...newService, name: e.target.value })
            }
          />

          <input
            placeholder="Duration (minutes)"
            className="border p-2 rounded"
            value={newService.durationMinutes}
            onChange={(e) =>
              setNewService({
                ...newService,
                durationMinutes: e.target.value,
              })
            }
          />

          <input
            placeholder="Price (kr)"
            className="border p-2 rounded"
            value={newService.price}
            onChange={(e) =>
              setNewService({ ...newService, price: e.target.value })
            }
          />
        </div>

        <button
          onClick={createService}
          className="mt-4 bg-purple-600 text-white px-4 py-2 rounded"
        >
          Add Service
        </button>
      </div>

      {/* Services List */}
      <div className="p-6 bg-white border rounded-lg shadow-sm">
        <h2 className="text-xl font-semibold mb-4">All Services</h2>

        {loading && <p>Loading services...</p>}

        {!loading && services.length === 0 && (
          <p className="text-gray-500">No services found.</p>
        )}

        <div className="space-y-4">
          {services.map((s) => (
            <div
              key={s.id}
              className="border p-4 rounded bg-gray-50 shadow-sm flex flex-col md:flex-row md:items-center md:justify-between gap-3"
            >
              {/* Display Mode */}
              {!editingService || editingService.id !== s.id ? (
                <>
                  <div>
                    <div className="font-medium">{s.name}</div>
                    <div className="text-sm text-gray-600">
                      Duration: {s.durationMinutes} min
                    </div>
                    <div className="text-sm text-gray-600">
                      Price: {s.price} kr
                    </div>
                  </div>

                  <div className="flex gap-3">
                    <button
                      onClick={() => setEditingService(s)}
                      className="px-3 py-1 bg-blue-500 text-white rounded"
                    >
                      Edit
                    </button>

                    <button
                      onClick={() => deleteService(s.id)}
                      className="px-3 py-1 bg-red-500 text-white rounded"
                    >
                      Delete
                    </button>
                  </div>
                </>
              ) : (
                /* Edit Mode */
                <div className="w-full space-y-3">
                  <input
                    className="border p-2 rounded w-full"
                    value={editingService.name}
                    onChange={(e) =>
                      setEditingService({
                        ...editingService,
                        name: e.target.value,
                      })
                    }
                  />

                  <input
                    className="border p-2 rounded w-full"
                    value={editingService.durationMinutes}
                    onChange={(e) =>
                      setEditingService({
                        ...editingService,
                        durationMinutes: e.target.value,
                      })
                    }
                  />

                  <input
                    className="border p-2 rounded w-full"
                    value={editingService.price}
                    onChange={(e) =>
                      setEditingService({
                        ...editingService,
                        price: e.target.value,
                      })
                    }
                  />

                  <div className="flex gap-3">
                    <button
                      onClick={updateService}
                      className="px-3 py-1 bg-green-600 text-white rounded"
                    >
                      Save
                    </button>

                    <button
                      onClick={() => setEditingService(null)}
                      className="px-3 py-1 bg-gray-400 text-white rounded"
                    >
                      Cancel
                    </button>
                  </div>
                </div>
              )}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
