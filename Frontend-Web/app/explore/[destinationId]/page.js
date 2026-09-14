import DestinationDetails from "@/components/ui/DestinationDetails";
import { Suspense } from "react";

export default async function DestinationDetailsPage({ params }) {
  const { destinationId } = params;

  return (
    <div>
      <Suspense fallback={<div>Loading...</div>}>
        <DestinationDetails destinationId={destinationId} />
      </Suspense>
    </div>
  );
}