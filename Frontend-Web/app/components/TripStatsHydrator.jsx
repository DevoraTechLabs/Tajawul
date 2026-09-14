"use client";

import { useEffect } from "react";
import useTripInteractionsStore from "@/store/TripInteractionsStore";

const TripStatsHydrator = ({ favoritesCount, wishesCount, clonesCount }) => {
  const { setAllCounts } = useTripInteractionsStore();

  useEffect(() => {
    setAllCounts({ favoritesCount, wishesCount, clonesCount });
  }, [favoritesCount, wishesCount, clonesCount, setAllCounts]);

  return null;
};

export default TripStatsHydrator;