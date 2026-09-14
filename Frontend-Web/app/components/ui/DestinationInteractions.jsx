"use client";

import { useEffect, useState } from "react";
import FavoriteBorderIcon from "@mui/icons-material/FavoriteBorder";
import FavoriteIcon from "@mui/icons-material/Favorite";
import CheckBoxOutlineBlankIcon from "@mui/icons-material/CheckBoxOutlineBlank";
import CheckBoxIcon from "@mui/icons-material/CheckBox";
import PersonAddAlt1Icon from "@mui/icons-material/PersonAddAlt1";
import PersonRemoveAlt1Icon from "@mui/icons-material/PersonRemoveAlt1";
import API from "@/utils/api";
import useAuthStore from "@/store/authStore";
import styles from "@/destination.module.css";
import { SmolGreenLoading } from "./Loading";
import Image from "next/image";
import useStatsStore from "@/store/statsStore"; // ✅ NEW: import Zustand store

const DestinationInteractions = ({ destinationId }) => {
  const [interactions, setInteractions] = useState({
    follow: false,
    visit: false,
    wish: false,
  });
  const [loading, setLoading] = useState(true);
  const { accessToken, roles } = useAuthStore();
  const [showProtectedPopup, setShowProtectedPopup] = useState(false);

  const { setWishesCount, setVisitorsCount, setFollowersCount } =
    useStatsStore(); // ✅ NEW: get update functions

  useEffect(() => {
    const fetchInteractionStatus = async () => {
      try {
        const response = await API.get(
          `/user/${destinationId}/destinationStatus`
        );
        if (response.data) {
          setInteractions({
            follow: !!response.data.follow,
            visit: !!response.data.visit,
            wish: !!response.data.wish,
          });
        }
      } catch (error) {
        console.error("Failed to fetch interaction status:", error);
      } finally {
        setLoading(false);
      }
    };

    if (accessToken) {
      fetchInteractionStatus();
    }
  }, [destinationId, accessToken]);

  const handleInteraction = async (type) => {
    if (!roles.includes("User")) {
      setShowProtectedPopup(true);
      return;
    }

    const currentValue = interactions[type];
    setInteractions((prev) => ({ ...prev, [type]: !currentValue }));

    try {
      const res = currentValue
        ? await API.delete(`/user/${destinationId}/${type}`)
        : await API.post(`/user/${destinationId}/${type}`);

      if (res?.data) {
        if (res.data.wishesCount !== undefined)
          setWishesCount(res.data.wishesCount);
        if (res.data.visitorsCount !== undefined)
          setVisitorsCount(res.data.visitorsCount);
        if (res.data.followersCount !== undefined)
          setFollowersCount(res.data.followersCount);
      }
    } catch (error) {
      console.error(`Failed to update ${type} status:`, error);
      setInteractions((prev) => ({ ...prev, [type]: currentValue })); // revert on fail
    }
  };

  if (loading) {
    return (
      <>
        <button className={styles.saveButton}>
          <SmolGreenLoading />
        </button>
        <button className={styles.saveButton}>
          <SmolGreenLoading />
        </button>
        <button className={styles.saveButton}>
          <SmolGreenLoading />
        </button>
      </>
    );
  }

  if (!accessToken) {
    return (
      <button
        className={styles.saveButton}
        onClick={() => (window.location.href = "/login")}
      >
        Sign in to interact
      </button>
    );
  }

  return (
    <>
      <button
        className={`${styles.saveButton} ${
          interactions.wish ? styles.activeButton : ""
        }`}
        onClick={() => handleInteraction("wish")}
      >
        {interactions.wish ? <FavoriteIcon /> : <FavoriteBorderIcon />}
        {interactions.wish ? "In Wishlist" : "Add To Wishlist"}
      </button>

      <button
        className={`${styles.saveButton} ${
          interactions.visit ? styles.activeButton : ""
        }`}
        onClick={() => handleInteraction("visit")}
      >
        {interactions.visit ? <CheckBoxIcon /> : <CheckBoxOutlineBlankIcon />}
        {interactions.visit ? "Visited" : "Visit"}
      </button>

      <button
        className={`${styles.saveButton} ${
          interactions.follow ? styles.activeButton : ""
        }`}
        onClick={() => handleInteraction("follow")}
      >
        {interactions.follow ? <PersonRemoveAlt1Icon /> : <PersonAddAlt1Icon />}
        {interactions.follow ? "Following" : "Follow"}
      </button>

      {showProtectedPopup && (
        <div className={styles.popupOverlay}>
          <div className={styles.popupContent}>
            <button
              className={styles.closeButton}
              onClick={() => setShowProtectedPopup(false)}
            >
              ✕
            </button>
            <div className={styles.completedStep}>
              <Image
                src="/protected-feature.svg"
                alt="Protected feature"
                width={450}
                height={350}
                className={styles.completedImage}
              />
              <button
                className={styles.ctaButton}
                onClick={() =>
                  (window.location.href = "/complete-your-profile")
                }
              >
                Complete Your Profile to Access
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default DestinationInteractions;
