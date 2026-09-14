import TripDetails from "@/components/ui/TripDetails";
import { Suspense } from "react";

export default function TripDetailsPage({ params }) {
  const tripId = params.tripId; // Make sure this matches your dynamic route segment name

  return (
    <div>
      <Suspense fallback={<div>Loading trip...</div>}>
        <TripDetails tripId={tripId} />
      </Suspense>
    </div>
  );
}