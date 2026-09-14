import { z } from "zod";

export const addTripSchema = z.object({
  title: z.string().min(3, "Title must be at least 3 characters long").max(100),
  description: z
    .string()
    .min(10, "Description must be at least 10 characters long")
    .max(500),

  priceRange: z.enum(["Low", "Mid", "Luxury"], {
    errorMap: () => ({
      message: "Price range must be either Low, Mid, or Luxury",
    }),
  }),

  tripDuration: z.enum(["Short", "Mid", "Long"], {
    errorMap: () => ({
      message: "Trip duration must be Short, Mid, or Long",
    }),
  }),

  visibility: z.enum(["Public", "Private", "Triphub"], {
    errorMap: () => ({
      message: "Visibility must be Public, Private, or Triphub",
    }),
  }),

  status: z.enum(["notStarted", "inProgress", "completed", "canceled"], {
    errorMap: () => ({
      message: "Status must be Not Started, In Progress, Completed, or Canceled",
    }),
  }),
  
  sameCountry: z.boolean().default(false),
});

