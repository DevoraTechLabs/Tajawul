"use client";

import { IconButton } from "@mui/material";
import countryFlags from "@/utils/countryFlags"; // We'll create this mapping

export default function CountryType({ country, isActive, onClick }) {
  const FlagComponent = countryFlags[country];

  return (
    <IconButton
      onClick={() => onClick(country)}
      sx={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        padding: "8px 12px",
        borderTopLeftRadius: "12px",
        borderTopRightRadius: "12px",
        borderBottomLeftRadius: "0px",
        borderBottomRightRadius: "0px",
        color: isActive
          ? "var(--Green-Hover)"
          : "var(--Neutrals-Medium-Outline)",
        fontFamily: '"DM Sans", sans-serif',
        borderBottom: isActive ? "2px solid var(--Green-Hover)" : "none",
        transition: "all 0.2s ease",
        "&:hover": {
          color: "var(--Neutrals-Black-Text)",
          borderBottom: "1px solid var(--Neutrals-Black-Text)",
          backgroundColor: "var(--Neutrals-Background)",
          "& span": {
            fontWeight: "700",
          },
        },
      }}
    >
      {FlagComponent && <FlagComponent style={{ fontSize: "1.5rem" }} />}
      <span
        style={{
          fontSize: "0.75rem",
          marginTop: "4px",
          fontFamily: '"DM Sans", sans-serif',
          fontWeight: isActive ? "700" : "400",
        }}
      >
        {country === "United Arab Emirates" ? "UAE" : country}
      </span>
    </IconButton>
  );
}
