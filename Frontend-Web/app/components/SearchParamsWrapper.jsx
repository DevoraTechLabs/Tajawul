"use client";

import { useSearchParams } from "next/navigation";
import { useEffect } from "react";

export default function SearchParamsWrapper({ setParams, paramKeys }) {
  const searchParams = useSearchParams();

  useEffect(() => {
    const extractedParams = {};
    paramKeys.forEach((key) => {
      extractedParams[key] = searchParams.get(key);
    });

    if (Object.values(extractedParams).every((val) => val !== null)) {
      setParams(extractedParams);
    }
  }, [searchParams, paramKeys, setParams]);

  return null; // ✅ No UI elements needed, only updates state
}
