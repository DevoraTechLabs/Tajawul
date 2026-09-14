import styles from "./PriceRange.module.css";

const PriceRange = ({ priceRange }) => {
  if (!priceRange) return null;

  const getPriceRangeDetails = () => {
    switch (priceRange.toLowerCase()) {
      case "low":
        return { text: "$ Low", className: styles.low };
      case "mid":
        return { text: "$$ Mid-Range", className: styles.mid };
      case "luxury":
        return { text: "$$$ Luxury", className: styles.luxury };
      default:
        return null;
    }
  };

  const details = getPriceRangeDetails();
  if (!details) return null;

  return (
    <div className={styles.priceRangeContainer}>
      <span className={`${styles.priceRange} ${details.className}`}>
        {/* Special handling for luxury gradient text */}
        {priceRange.toLowerCase() === "luxury" ? (
          <span>{details.text}</span>
        ) : (
          details.text
        )}
      </span>
    </div>
  );
};

export default PriceRange;