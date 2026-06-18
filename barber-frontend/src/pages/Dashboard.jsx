export default function Dashboard() {
    return (
        <div className="p-10 text-2xl font-semibold">
            Welcome to the Barber System Dashboard

        <section>
            <a
                href="/booking/services"
                className="inline-block bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700"
                >
                Start Booking
            </a>
        </section>
        </div>
        
    );
}
