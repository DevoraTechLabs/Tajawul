"use client";

import { usePathname } from "next/navigation";
import { useEffect } from "react";

export default function TripIdHandler({ tripId }) {
  const pathname = usePathname();

  useEffect(() => {
    if (tripId) {
      localStorage.setItem("tripId", tripId);
    }

    return () => {
      // Clean up when component unmounts or path changes
      if (pathname.startsWith("/tribhub/")) {
        localStorage.removeItem("tripId");
      }
    };
  }, [tripId, pathname]);

  return null;
}