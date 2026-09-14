"use client";

import React, { useEffect, useState } from "react";
import Image from "next/image";
import Link from "next/link";
import API from "@/utils/api";
import styles from "@/components/ui/DestinationCard.module.css";
import Rating from "@/components/ui/Rating";
import PriceRange from "@/components/ui/tags/PriceRange";
import PlaceIcon from "@mui/icons-material/Place";
import typeIconsMapping from "@/utils/typeIconsMapping";
import { GreenLoading } from "@/components/ui/Loading";

const RecommendedDest = () => {
  const [destinations, setDestinations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRecommendedDestinations = async () => {
      try {
        const response = await API.get("/Recommendation/destinations");
        setDestinations(response.data.items || []);
      } catch (err) {
        console.error("Failed to fetch recommended destinations:", err);
        setError("Unable to load recommendations at this time.");
      } finally {
        setLoading(false);
      }
    };

    fetchRecommendedDestinations();
  }, []);

  if (loading) {
    return (
      <div className={styles.recommendedSection}>
        <h2 className={styles.recommendedTitle}>
          Recommended Destinations For You
        </h2>
        <div style={{ margin: "40px 0" }}>
          <GreenLoading />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={styles.recommendedSection}>
        <h2 className={styles.recommendedTitle}>
          Recommended Destinations For You
        </h2>
        <p className={styles.recommendedError}>{error}</p>
      </div>
    );
  }

  return (
    <div className={styles.recommendedSection}>
      <h2 className={styles.recommendedTitle}>
        Recommended Destinations For You
      </h2>
      <p className={styles.recommendedSubtitle}>
        Handpicked just for you — these destinations match your travel style and
        interests, making every journey feel more personal and unforgettable.
      </p>
      <div className={styles.recommendedList}>
        {destinations.map((destination) => {
          const IconComponent =
            typeIconsMapping[destination.type] || typeIconsMapping.Explore;

          return (
            <div key={destination.destinationId} className={styles.card}>
              <Link href={`/explore/${destination.destinationId}`}>
                <div className={styles.imageWrapper}>
                  <Image
                    src={destination.coverImage || "/fallback.jpg"}
                    alt={destination.name}
                    className={styles.image}
                    width={290}
                    height={430}
                    onError={(e) => {
                      e.currentTarget.src = "/fallback.jpg";
                    }}
                  />
                  <div className={styles.topContainer}>
                    <PriceRange priceRange={destination.priceRange} />
                  </div>
                  <div className={styles.locationTag}>
                    <PlaceIcon style={{ fontSize: "18px" }} />
                    <p>
                      {destination.city}, {destination.country}
                    </p>
                  </div>
                </div>
                <div className={styles.content}>
                  <div className={styles.top}>
                    <span className={styles.name}>{destination.name}</span>
                    <div className={styles.description}>
                      <div className={styles.typeIcon}>
                        <IconComponent style={{ fontSize: "18px" }} />
                      </div>
                      {destination.type}
                    </div>
                    <div className={styles.rating}>
                      <Rating average={destination.averageRating} />
                    </div>
                  </div>
                </div>
              </Link>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default RecommendedDest;
