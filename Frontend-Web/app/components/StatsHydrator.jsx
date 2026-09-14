"use client";

import { useEffect } from "react";
import useStatsStore from "@/store/statsStore";

const StatsHydrator = ({ wishesCount, visitorsCount, followersCount }) => {
  const setAllCounts = useStatsStore((state) => state.setAllCounts);

  useEffect(() => {
    setAllCounts({ wishesCount, visitorsCount, followersCount });
  }, [wishesCount, visitorsCount, followersCount, setAllCounts]);

  return null;
};

export default StatsHydrator;
