import React from "react";
import styles from "./Tag.module.css";
import {
  SportsSoccer as SportsIcon,
  Restaurant as FoodIcon,
  LocalBar as DrinkIcon,
  Terrain as NatureIcon,
  Museum as CultureIcon,
  ShoppingBag as ShoppingIcon,
  BeachAccess as BeachIcon,
  DirectionsCar as TransportationIcon,
  Hotel as AccommodationIcon,
  Festival as EventsIcon,
  Diamond as HiddenGemsIcon, // Added for Hidden Gems
  LocationCity as UrbanIcon, // Added for Urban
  PhotoCamera as PhotographyIcon, // Added for Photography
  HelpOutline as DefaultIcon, // Fallback icon
} from "@mui/icons-material";

const Tag = ({ options = null }) => {
  // Return null if options is null, undefined, or not an array
  if (!options || !Array.isArray(options)) {
    return null;
  }

  // Filter out any null/undefined values from the options array
  const validOptions = options.filter((option) => option != null);

  // Return null if there are no valid options after filtering
  if (validOptions.length === 0) {
    return null;
  }

  // Function to get icon based on text content
  const getIconForTag = (text) => {
    if (!text) return;

    const lowerText = text.toLowerCase();

    if (lowerText.includes("sport"))
      return <SportsIcon className={styles.icon} />;
    if (lowerText.includes("food") || lowerText.includes("restaurant"))
      return <FoodIcon className={styles.icon} />;
    if (lowerText.includes("drink") || lowerText.includes("bar"))
      return <DrinkIcon className={styles.icon} />;
    if (lowerText.includes("nature") || lowerText.includes("park"))
      return <NatureIcon className={styles.icon} />;
    if (lowerText.includes("culture") || lowerText.includes("museum"))
      return <CultureIcon className={styles.icon} />;
    if (lowerText.includes("shop"))
      return <ShoppingIcon className={styles.icon} />;
    if (lowerText.includes("beach"))
      return <BeachIcon className={styles.icon} />;
    if (lowerText.includes("transport"))
      return <TransportationIcon className={styles.icon} />;
    if (lowerText.includes("hotel") || lowerText.includes("accommodation"))
      return <AccommodationIcon className={styles.icon} />;
    if (lowerText.includes("event") || lowerText.includes("festival"))
      return <EventsIcon className={styles.icon} />;
    if (lowerText.includes("hidden gem") || lowerText.includes("hidden gems"))
      return <HiddenGemsIcon className={styles.icon} />;
    if (lowerText.includes("urban"))
      return <UrbanIcon className={styles.icon} />;
    if (lowerText.includes("photography"))
      return <PhotographyIcon className={styles.icon} />;

    return;
  };

  // Capitalize first letter of each word
  const formatText = (text) => {
    if (!text) return "";
    return text
      .split(" ")
      .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
      .join(" ");
  };

  return (
    <div className={styles.tagsContainer}>
      {validOptions.map((option, index) => (
        <div key={index} className={styles.tag}>
          {getIconForTag(option)}
          <span className={styles.tagText}>{formatText(option)}</span>
        </div>
      ))}
    </div>
  );
};

export default Tag;