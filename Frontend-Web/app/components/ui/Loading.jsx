"use client";

export default function Loading() {
  return (
    <div className="overlay">
      <div className="spinner"></div>
    </div>
  );
}

export function GreenLoading() {
  return (
    <div
      style={{
        width: "100%",
        height: "100%",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <div
        className="spinner"
        style={{
          borderColor: "rgba(92, 156, 146, 0.3)",
          borderTopColor: "var(--Green-Hover)",
        }}
      ></div>
    </div>
  );
}

export function SmolGreenLoading() {
  return (
    <div
      style={{
        width: "100%",
        height: "100%",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <div
        className="spinner"
        style={{
          width: "25px",
          height: "25px",
          borderWidth: "4px", // Increased from 3px
          borderColor: "rgba(92, 156, 146, 0.3)",
          borderTopColor: "var(--Green-Hover)",
          borderRadius: "50%", // Ensures perfect circle
          animation: "spin 1s linear infinite", // Ensure animation is applied
        }}
      ></div>
    </div>
  );
}

export function WhiteLoading() {
  return (
    <div
      style={{
        width: "100%",
        height: "100%",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <div
        className="spinner"
        style={{
          width: "22px",
          height: "22px",
          borderWidth: "4px", // Increased from 3px
          borderColor: "rgba(255, 251, 244, 0.3)",
          borderTopColor: "var(--Neutrals-Background)",
          borderRadius: "50%", // Ensures perfect circle
          animation: "spin 1s linear infinite", // Ensure animation is applied
        }}
      ></div>
    </div>
  );
}
