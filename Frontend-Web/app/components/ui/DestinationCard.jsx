"use client";

import React, { useState } from "react";
import styles from "./DestinationCard.module.css";
import Rating from "./Rating";
import PriceRange from "./tags/PriceRange";
import PlaceIcon from "@mui/icons-material/Place";
import Image from "next/image";

const DestinationCard = ({
  image,
  name,
  type,
  location,
  typeIcon,
  rating,
  ratingCount,
  priceRange,
}) => {
  const [imageSrc, setImageSrc] = useState(image || "/fallback.jpg"); // Fallback image if image is empty
  const fallbackImage = "/fallback.jpg"; // Path to your fallback image

  const handleImageError = () => {
    // If the image fails to load, set the fallback image
    setImageSrc(fallbackImage);
  };

  return (
    <div className={styles.card}>
      <div className={styles.imageWrapper}>
        {imageSrc && (
          <Image
            src={imageSrc}
            alt={name}
            className={styles.image}
            width={290}
            height={430}
            onLoad={(event) => {
              const img = event.currentTarget;
              if (img.naturalWidth === 0) {
                // If the image fails to load, set the fallback image
                handleImageError();
              }
            }}
            onError={handleImageError} // Fallback for additional error handling
          />
        )}
        <div className={styles.topContainer}>
          <PriceRange priceRange={priceRange} />
        </div>

        <div className={styles.locationTag}>
          <PlaceIcon style={{ fontSize: "18px" }} /> <p>{location}</p>
        </div>
      </div>
      <div className={styles.content}>
        <div className={styles.top}>
          <span className={styles.name}>{name}</span>
          <div className={styles.description}>
            {typeIcon &&
              React.createElement(typeIcon, { style: { fontSize: "18px" } })}
            {type}
          </div>
          <div className={styles.rating}>
            <Rating average={rating} /> <span>{ratingCount}</span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DestinationCard;
