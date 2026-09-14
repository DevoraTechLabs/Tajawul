import React from "react";
import AccessTimeIcon from "@mui/icons-material/AccessTime";
import styles from "./OpenClose.module.css";

const OpenClose = ({ openTime, closeTime, isOpen24Hours }) => {
  // ✅ Extract only "hh:mm" from the provided time string and format it to 12-hour AM/PM format
  const formatTime = (timeStr) => {
    if (!timeStr) return "N/A";
    const [hours, minutes] = timeStr.split(":").map(Number);
    const date = new Date();
    date.setHours(hours, minutes, 0, 0);
    return date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
  };

  const parseTime = (timeStr) => {
    if (!timeStr) return null;
    const [hours, minutes] = timeStr.split(":").map(Number);
    return new Date().setHours(hours, minutes, 0, 0);
  };

  const now = new Date();
  const open = parseTime(openTime);
  const close = parseTime(closeTime);

  // Handle next-day closing times (when close time is before open time)
  let isOpen;
  if (isOpen24Hours) {
    isOpen = true;
  } else if (open && close) {
    if (close > open) {
      // Normal case: closes same day
      isOpen = now >= open && now <= close;
    } else {
      // Special case: closes next day (open overnight)
      isOpen = now >= open || now <= close;
    }
  } else {
    isOpen = false;
  }

  return (
    <div className={styles.timeContainer}>
      <div className={isOpen ? styles.open : styles.closed}>
        <AccessTimeIcon />
        {isOpen24Hours
          ? "Open 24 Hours"
          : isOpen
          ? `Open until ${formatTime(closeTime)}`
          : `Closed Now`}
      </div>
      {!isOpen24Hours && (
        <span>
          • {formatTime(openTime)} - {formatTime(closeTime)}
        </span>
      )}
    </div>
  );
};

export default OpenClose;
