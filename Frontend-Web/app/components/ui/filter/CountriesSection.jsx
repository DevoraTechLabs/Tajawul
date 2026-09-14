"use client";

import { useState, useRef } from "react";
import { IconButton, Box } from "@mui/material";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import CountryType from "./CountryType";
import arabCountries from "@/data/arabCountries.json";

export default function CountriesSection({
  onCountrySelect,
  selectedCountry: propSelectedCountry,
}) {
  const [internalSelectedCountry, setInternalSelectedCountry] = useState(null);
  const scrollContainerRef = useRef(null);

  // Extract and sort country names alphabetically
  const countries = Object.keys(arabCountries).sort((a, b) =>
    a.localeCompare(b)
  );

  // Use the prop if provided, otherwise use internal state
  const selectedCountry =
    propSelectedCountry !== undefined
      ? propSelectedCountry
      : internalSelectedCountry;

  const scroll = (scrollOffset) => {
    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollLeft += scrollOffset;
    }
  };

  const handleCountryClick = (country) => {
    const newSelectedCountry = country === selectedCountry ? null : country;
    if (onCountrySelect) {
      onCountrySelect(newSelectedCountry);
    } else {
      setInternalSelectedCountry(newSelectedCountry);
    }
  };

  return (
    <Box sx={{ display: "flex", alignItems: "center", gap: 1, width: "100%" }}>
      <IconButton
        onClick={() => scroll(-200)}
        sx={{
          border: "1px solid var(--Neutrals-Light-Outline)",
          transition: "all 0.2s ease",
          padding: "4px",
          "&:hover": {
            borderColor: "var(--Neutrals-Black-Text)",
          },
          "& .MuiSvgIcon-root": {
            fontSize: "1.2rem",
          },
        }}
      >
        <ChevronLeftIcon sx={{ color: "var(--Neutrals-Black-Text)" }} />
      </IconButton>

      <Box
        ref={scrollContainerRef}
        sx={{
          display: "flex",
          width: "100%",
          maxWidth: "calc(100% - 80px)",
          overflowX: "auto",
          scrollBehavior: "smooth",
          gap: 1,
          "&::-webkit-scrollbar": {
            display: "none",
          },
          msOverflowStyle: "none",
          scrollbarWidth: "none",
        }}
      >
        {countries.map((country) => (
          <CountryType
            key={country}
            country={country}
            isActive={country === selectedCountry}
            onClick={handleCountryClick}
          />
        ))}
      </Box>

      <IconButton
        onClick={() => scroll(200)}
        sx={{
          border: "1px solid var(--Neutrals-Light-Outline)",
          transition: "all 0.2s ease",
          padding: "4px",
          "&:hover": {
            borderColor: "var(--Neutrals-Black-Text)",
          },
          "& .MuiSvgIcon-root": {
            fontSize: "1.2rem",
          },
        }}
      >
        <ChevronRightIcon sx={{ color: "var(--Neutrals-Black-Text)" }} />
      </IconButton>
    </Box>
  );
}
