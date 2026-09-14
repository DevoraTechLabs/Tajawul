import ExplorePageClient from "./ExplorePageClient";

async function getDestinations() {
  const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
  const url = `${baseUrl}/Destination?PageNumber=1&PageSize=50`;

  const res = await fetch(url, { next: { revalidate: 60 } }); // Changed to 1 minute revalidation

  if (!res.ok) {
    throw new Error("Failed to fetch data");
  }

  return res.json();
}

export default async function ExplorePage() {
  const initialDestinations = await getDestinations();

  return <ExplorePageClient initialDestinations={initialDestinations} />;
}
