"use client";

import { usePathname } from "next/navigation";
import { useEffect } from "react";

export default function DestinationIdHandler({ destinationId }) {
  const pathname = usePathname();

  useEffect(() => {
    // Store destinationId in localStorage when component mounts
    if (destinationId) {
      localStorage.setItem("destinationId", destinationId);
    }

    return () => {
      // Clean up when component unmounts or path changes
      if (pathname.startsWith("/explore/")) {
        localStorage.removeItem("destinationId");
      }
    };
  }, [destinationId, pathname]);

  return null;
}
