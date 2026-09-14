"use client";

import React from "react";
import FavoriteIcon from "@mui/icons-material/Favorite";
import BookmarkBorderIcon from "@mui/icons-material/BookmarkBorder";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import styles from "./Stats.module.css";
import useTripInteractionsStore from "@/store/TripInteractionsStore";

const Stats = ({ type }) => {
  const { favoritesCount, wishesCount, clonesCount } = useTripInteractionsStore();

  const icons = {
    Favorites: <FavoriteIcon sx={{ fontSize: 32, color: "var(--Red-Error)" }} />,
    Wishes: <BookmarkBorderIcon sx={{ fontSize: 32, color: "var(--Red-Error)" }} />,
    Clones: <ContentCopyIcon sx={{ fontSize: 32, color: "var(--Red-Error))" }} />,
  };

  const getCount = () => {
    switch (type) {
      case "Favorites": 
        return favoritesCount;
      case "Wishes": 
        return wishesCount;
      case "Clones": 
        return clonesCount;
      default: 
        return 0;
    }
  };

  const formatCount = (num) => {
    if (num >= 1000) return (num / 1000).toFixed(0) + "k";
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

