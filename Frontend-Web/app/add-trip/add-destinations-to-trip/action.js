import { z } from "zod";

export const addDestinationToTripSchema = z.object({
  destinationId: z.string().min(1, "Destination ID is required"),
  day: z.number().min(1, "Day must be at least 1")
});