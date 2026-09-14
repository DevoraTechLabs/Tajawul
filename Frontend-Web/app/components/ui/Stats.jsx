"use client";

import React from "react";
import PeopleIcon from "@mui/icons-material/People";
import WhereToVoteIcon from "@mui/icons-material/WhereToVote";
import FavoriteIcon from "@mui/icons-material/Favorite";
import styles from "./Stats.module.css";
import useStatsStore from "@/store/statsStore"; // ✅ Zustand import

const Stats = ({ type }) => {
  const { wishesCount, visitorsCount, followersCount } = useStatsStore(); // 🧠 Subscribed live values

  const icons = {
    Followers: <PeopleIcon sx={{ fontSize: 32 }} />,
    Visitors: <WhereToVoteIcon sx={{ fontSize: 38 }} />,
    Wishes: <FavoriteIcon sx={{ fontSize: 32 }} />,
  };

  const getCount = () => {
    switch (type) {
      case "Wishes":
        return wishesCount;
      case "Visitors":
        return visitorsCount;
      case "Followers":
        return followersCount;
      default:
        return 0;
    }
  };

  const formatCount = (num) => {
    return num >= 1000 ? (num / 1000).toFixed(0) + "K" : num;
  };

  return (
    <div className={styles.container}>
      <div className={styles.iconBox}>{icons[type]}</div>
      <p className={styles.count}>{formatCount(getCount())}</p>
      <p className={styles.type}>{type}</p>
    </div>
  );
};

export default Stats;
