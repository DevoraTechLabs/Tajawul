import TripsHubClient from "./TripsHubClient";

async function getTrips() {
  const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
  const res = await fetch(`${baseUrl}/Trip?Visibility=Triphub`, {
    next: { revalidate: 60 } // Revalidate every 60 seconds
  });

  if (!res.ok) {
    throw new Error(`Failed to fetch trips (Status: ${res.status})`);
  }

  const data = await res.json();
  console.log('Server-side trips data:', data); // Add this line
  return data;
}

export default async function TripsHubPage() {
  const initialTrips = await getTrips();
  return <TripsHubClient initialTrips={initialTrips} />;
}