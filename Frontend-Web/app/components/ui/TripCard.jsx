"use client";

import React, { useState } from "react";
import styles from "./TripCard.module.css";
import PriceRange from "./tags/PriceRange";
import BookmarkBorderIcon from "@mui/icons-material/BookmarkBorder";
import Image from "next/image";
import PublicIcon from "@mui/icons-material/Public";
import AccessTimeIcon from "@mui/icons-material/AccessTime";
import Link from "next/link";

const TripCard = ({ trip }) => {
  const [imageSrc, setImageSrc] = useState(trip?.coverImage || "/fallback.jpg");
  const fallbackImage = "/fallback.jpg";

  const handleWishlist = (e) => {
    e.stopPropagation();
    alert(`Added ${trip?.title} to wishlist!`);
  };

  const handleImageError = () => {
    setImageSrc(fallbackImage);
  };

  const getDurationClass = () => {
    if (!trip?.tripDuration) return styles.durationUnknown;
    
    const duration = trip.tripDuration.toLowerCase();
    
    switch(duration) {
      case "short": return styles.durationShort;
      case "mid": return styles.durationMid;
      case "long": return styles.durationLong;
      default: return styles.durationUnknown;
    }
  };

  const getDurationText = () => {
    if (!trip?.tripDuration) return "Duration not specified";
    
    const duration = trip.tripDuration.toLowerCase();
    
    switch(duration) {
      case "short": return "1-3 days (Short)";
      case "mid": return "4-7 days (Mid)";
      case "long": return "8+ days (Long)";
      default: return "Duration not specified";
    }
  };

  const getCountryClass = () => {
    if (trip?.sameCountry === true) return styles.countrySame;
    if (trip?.sameCountry === false) return styles.countryDifferent;
    return styles.countryUnknown;
  };

  const getCountryText = () => {
    if (trip?.sameCountry === true) return "Same Country";
    if (trip?.sameCountry === false) return "Different Countries";
    return "Country info not available";
  };

  return (
    <Link href={`/triphub/${trip.tripId || trip.id}`} passHref>
      <div className={styles.card}>
        <div className={styles.imageWrapper}>
          <Image
            src={imageSrc}
            alt={trip?.title || "Trip image"}
            className={styles.image}
            width={400}
            height={400}
            quality={90}
            priority={false}
            loading="eager"
            sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 400px"
            onLoad={(event) => {
              const img = event.currentTarget;
              if (img.naturalWidth === 0) {
                handleImageError();
              }
            }}
            onError={handleImageError}
          />
          <div className={styles.topContainer}>
            <PriceRange priceRange={trip?.priceRange} />
            <button 
              className={styles.wishlistButton} 
              onClick={handleWishlist}
              aria-label="Add to wishlist"
            >
              <BookmarkBorderIcon className={styles.wishlistIcon} />
            </button>
          </div>
        </div>
        <div className={styles.content}>
          <div className={styles.top}>
            <span className={styles.name}>{trip?.title || "Untitled Trip"}</span>
            <div className={styles.infoContainer}>
              <div className={`${styles.infoItem} ${getDurationClass()}`}>
                <AccessTimeIcon className={styles.infoIcon} />
                <span>{getDurationText()}</span>
              </div>
              <div className={`${styles.infoItem} ${getCountryClass()}`}>
                <PublicIcon className={styles.infoIcon} />
                <span>{getCountryText()}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Link>
  );
};

export default TripCard;