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

const TopRatingDest = () => {
  const [destinations, setDestinations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchTopRatedDestinations = async () => {
      try {
        const response = await API.get(
          "/Destination?sortBy=AverageRating&Ascending=false&PageNumber=1&PageSize=5"
        );
        setDestinations(response.data.destinations || []);
      } catch (err) {
        console.error("Failed to fetch top rated destinations:", err);
        setError("Unable to load top rated destinations at this time.");
      } finally {
        setLoading(false);
      }
    };

    fetchTopRatedDestinations();
  }, []);

  if (loading) {
    return (
      <div className={styles.recommendedSection}>
        <h2 className={styles.recommendedTitle}>Top Rated Destinations</h2>
        <div style={{ margin: "40px 0" }}>
          <GreenLoading />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={styles.recommendedSection}>
        <h2 className={styles.recommendedTitle}>Top Rated Destinations</h2>
        <p className={styles.recommendedError}>{error}</p>
      </div>
    );
  }

  return (
    <div className={styles.recommendedSection}>
      <h2 className={styles.recommendedTitle}>Top Rated Destinations</h2>
      <p className={styles.recommendedSubtitle}>
        Discover the top 5 must-visit destinations, ranked by traveler ratings
        and loved by the Tajawul community.
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
                      <span>{destination.reviewsCount} Reviews</span>
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

export default TopRatingDest;
