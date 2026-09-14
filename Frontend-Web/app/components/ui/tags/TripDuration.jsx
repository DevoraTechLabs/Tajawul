import styles from "./TripDuration.module.css";
import QueryBuilderIcon from '@mui/icons-material/QueryBuilder';

const TripDuration = ({ durations = [] }) => {
  if (!durations || !Array.isArray(durations)) return null;

  const validDurations = durations.filter(
    (duration) => duration != null && duration.trim() !== ""
  );

  if (validDurations.length === 0) return null;

  const getDurationDetails = (duration) => {
    if (!duration) return null;

    const lowerDuration = duration.toLowerCase();

    switch (lowerDuration) {
      case "short":
        return {
          text: "Short",
          icon: <QueryBuilderIcon fontSize="small" />,
          className: styles.short
        };
      case "mid":
      case "medium":
        return {
          text: "Mid",
          icon: <QueryBuilderIcon fontSize="small" />,
          className: styles.mid
        };
      case "long":
        return {
          text: "Long",
          icon: <QueryBuilderIcon fontSize="small" />,
          className: styles.long
        };
      default:
        return null;
    }
  };

  return (
    <div className={styles.durationContainer}>
      {validDurations.map((duration, index) => {
        const details = getDurationDetails(duration);
        if (!details) return null;

        return (
          <span key={index} className={`${styles.tag} ${details.className}`}>
            <span className={styles.iconTextWrapper}>
              {details.icon}
              {details.text}
            </span>
          </span>
        );
      })}
    </div>
  );
};

export default TripDuration;