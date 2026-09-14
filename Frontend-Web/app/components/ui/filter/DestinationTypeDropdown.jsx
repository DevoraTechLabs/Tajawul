"use client";

import { useState } from "react";
import { Menu, MenuItem, Button, Box } from "@mui/material";
import typeIconsMapping from "@/utils/typeIconsMapping";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";

export default function DestinationTypeDropdown({
  selectedType,
  onTypeSelect,
}) {
  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleTypeSelect = (type) => {
    onTypeSelect(type);
    handleClose();
  };

  // Get the icon component for the selected type
  const SelectedIcon = selectedType ? typeIconsMapping[selectedType] : null;

  // Sort the types alphabetically
  const sortedTypes = Object.keys(typeIconsMapping).sort();

  return (
    <Box sx={{ position: "relative" }}>
      <Button
        className="typesDropdown"
        onClick={handleClick}
        sx={{
          display: "flex",
          alignItems: "center",
          gap: "8px",
          padding: "8px 16px",
          height: "34px",
          border: "none",
          borderRadius: "8px",
          cursor: "pointer",
          fontFamily: '"DM Sans"',
          fontWeight: "500",
          letterSpacing: "0",
          color: "var(--Neutrals-Background)",
          backgroundColor: "var(--Neutrals-Black-Text)",
          transition: "all 0.3s ease-in-out",
          textTransform: "capitalize",
          "&:hover": {
            backgroundColor: "var(--Neutrals-Medium-Outline)",
          },
        }}
      >
        {SelectedIcon && <SelectedIcon sx={{ fontSize: "18px" }} />}
        {selectedType || "All"}
        <KeyboardArrowDownIcon
          sx={{
            transition: "transform 0.3s ease-in-out",
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
            width: "200px",
            padding: "4px",
            "& .MuiMenuItem-root": {
              borderRadius: "8px",
              border: "1px solid #FFF",
              padding: "8px 10px",
              fontFamily: '"DM Sans"',
              fontWeight: "500",
              fontSize: "14px",
              color: "var(--Neutrals-Medium-Outline)",
              transition: "all 0.2s ease-in-out",
              "&:hover, &.Mui-focusVisible": {
                backgroundColor: "var(--Beige-Very-Bright)",
                color: "var(--Neutrals-Black-Text)",
                border: "1px solid var(--Neutrals-Light-Outline)",
              },
            },
          },
        }}
        PaperProps={{
          sx: {
            borderRadius: "12px",
            border: "1px solid var(--Neutrals-Light-Outline)",
            boxShadow: "0px 4px 8px rgba(0, 0, 0, 0.1)",
            marginTop: "4px",
          },
        }}
      >
        <MenuItem onClick={() => handleTypeSelect(null)}>
          <Box sx={{ display: "flex", alignItems: "center", gap: "8px" }}>
            All
          </Box>
        </MenuItem>
        {sortedTypes.map((type) => {
          const IconComponent = typeIconsMapping[type];
          return (
            <MenuItem key={type} onClick={() => handleTypeSelect(type)}>
              <Box sx={{ display: "flex", alignItems: "center", gap: "8px" }}>
                {IconComponent && <IconComponent fontSize="small" />}
                {type}
              </Box>
            </MenuItem>
          );
        })}
      </Menu>
    </Box>
  );
}
