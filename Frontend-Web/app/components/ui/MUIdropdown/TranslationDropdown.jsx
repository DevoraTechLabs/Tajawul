"use client";

import { useState } from "react";
import { Menu, MenuItem, Button, Box } from "@mui/material";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import LanguageIcon from "@mui/icons-material/Language";
import languages from "@/data/frontend_translation_languages.json";

export default function TranslationDropdown({
  selectedLanguage,
  onLanguageSelect,
}) {
  const [anchorEl, setAnchorEl] = useState(null);
  const [menuWidth, setMenuWidth] = useState(null);
  const open = Boolean(anchorEl);

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
    setMenuWidth(event.currentTarget.offsetWidth);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleLanguageSelect = (languageName) => {
    const languageCode = languages[languageName];
    onLanguageSelect(languageName, languageCode);
    handleClose();
  };

  return (
    <Box sx={{ position: "relative", width: "100%" }}>
      <Button
        className="languageDropdown"
        onClick={handleClick}
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          width: "100%",
          height: "46px",
          gap: "8px",
          padding: "8px 24px",
          borderRadius: "42px",
          cursor: "pointer",
          fontFamily: '"Merriweather"',
          fontSize: "16px",
          fontWeight: "600",
          letterSpacing: "0",
          textTransform: "capitalize",
          transition: "all 0.2s ease-in-out",
          border: open
            ? "1.5px solid var(--Neutrals-Light-Outline)"
            : "1.5px solid var(--Neutrals-Light-Outline)",
          borderBottom: open
            ? "1px solid var(--Neutrals-Light-Outline)"
            : "1.5px solid var(--Neutrals-Light-Outline)",
          backgroundColor: open ? "var(--Neutrals-Background)" : "#f0dcbeeb",
          color: open
            ? "var(--Neutrals-Medium-Outline)"
            : "var(--Neutrals-Black-Text)",
          boxShadow: open ? "0px 4px 10px 0px rgba(34, 26, 20, 0.15)" : "none",
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
          <LanguageIcon sx={{ fontSize: "18px" }} />
          {selectedLanguage || "Choose Language"}
        </div>
        <KeyboardArrowDownIcon
          sx={{
            transition: "transform 0.2s ease-in-out",
            transform: open ? "rotate(180deg)" : "rotate(0)",
            fontSize: "18px",
          }}
        />
      </Button>
      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        MenuListProps={{
          "aria-labelledby": "basic-button",
          sx: {
            maxHeight: 400,
            display: "flex",
            alignItems: "flex-start",
            alignSelf: "stretch",
            flexDirection: "column",
            gap: "0px",
            width: "100%",
            padding: "8px",
            "& .MuiMenuItem-root": {
              width: "100%",
              borderRadius: "8px",
              border: "1px solid #FFF",
              padding: "8px 16px",
              fontFamily: '"DM Sans"',
              fontWeight: "500",
              fontSize: "16px",
              color: "var(--Neutrals-Medium-Outline)",
              transition: "all 0.2s ease-in-out",
              "&:hover, &.Mui-focusVisible": {
                backgroundColor: "var(--Beige-Very-Bright)",
                color: "var(--Neutrals-Black-Text)",
                border: "1px solid var(--Neutrals-Light-Outline)",
              },
              "&.Mui-selected": {
                border: "1px solid var(--Neutrals-Medium-Outline)",
                backgroundColor: "var(--Beige-Hover)",
                fontWeight: "600",
                color: "var(--Neutrals-Black-Text)",
              },
            },
          },
        }}
        PaperProps={{
          sx: {
            width: menuWidth || "auto",
            borderRadius: "12px",
            border: "1px solid var(--Neutrals-Light-Outline)",
            boxShadow: "0px 4px 8px rgba(0, 0, 0, 0.1)",
            marginTop: "4px",
          },
        }}
      >
        {Object.keys(languages)
          .sort((a, b) => a.localeCompare(b))
          .map((languageName) => (
            <MenuItem
              key={languages[languageName]}
              selected={languageName === selectedLanguage}
              onClick={() => handleLanguageSelect(languageName)}
            >
              <Box sx={{ display: "flex", alignItems: "center", gap: "8px" }}>
                {languageName}
              </Box>
            </MenuItem>
          ))}
      </Menu>
    </Box>
  );
}
