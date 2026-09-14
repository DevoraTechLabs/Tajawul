"use client";
import { useState, useRef } from "react";
import { Menu, MenuItem, Button, Box } from "@mui/material";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";

export default function Dropdown({
  label,
  id,
  required,
  value,
  onChange,
  options,
  description,
  errorMsg,
  disabled = false,
  sortDirection = "ascending",
}) {
  const [anchorEl, setAnchorEl] = useState(null);
  const dropdownRef = useRef(null);
  const open = Boolean(anchorEl);

  // Sort options
  const sortedOptions = [...options].sort((a, b) => {
    const comparison = a.label.localeCompare(b.label);
    return sortDirection === "ascending" ? comparison : -comparison;
  });

  const handleClick = (event) => {
    if (!disabled) {
      setAnchorEl(event.currentTarget);
    }
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleSelect = (selectedValue) => {
    onChange({ target: { name: id, value: selectedValue } });
    handleClose();
  };

  const selectedLabel = options.find((opt) => opt.value === value)?.label || "";

  return (
    <Box
      sx={{ position: "relative", width: "100%", mb: "12px" }}
      ref={dropdownRef}
    >
      {/* Label and description section */}
      <Box
        component="label"
        sx={{
          color: "var(--Neutrals-Black-Text, #221a14)",
          fontFamily: "var(--font-heading), Georgia, serif",
          fontSize: "16px",
          fontWeight: "700",
          lineHeight: "120%",
          width: "100%",
          textAlign: "left",
          mb: "6px",
          display: "block",
        }}
        htmlFor={id}
      >
        {label}{" "}
        {required && (
          <span
            style={{
              color: "#ef4444",
              fontFamily: "var(--font-body), system-ui, sans-serif",
              letterSpacing: "0.01em",
            }}
          >
            *
          </span>
        )}
        {description && (
          <span
            style={{
              fontFamily: "var(--font-body), system-ui, sans-serif",
              letterSpacing: "0.01em",
              fontSize: "0.875rem",
              fontWeight: "400",
              color: "var(--Neutrals-Medium-Outline)",
              display: "block",
              mt: "2px",
            }}
          >
            {description}
          </span>
        )}
      </Box>

      {/* Button section */}
      <Button
        id={id}
        aria-haspopup="listbox"
        aria-expanded={open}
        aria-controls={`${id}-menu`}
        onClick={handleClick}
        disabled={disabled}
        disableRipple
        sx={{
          position: "relative",
          width: "100%",
          height: "46px",
          padding: "16px 20px",
          borderRadius: "12px",
          border: "1px solid var(--Neutrals-Light-Outline)",
          backgroundColor: "#fff",
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          cursor: "pointer",
          transition: "border-color 0.2s ease",
          textTransform: "none",
          "&:hover": {
            borderColor: "var(--Neutrals-Black-Text)",
            backgroundColor: "#fff",
          },
          "&.Mui-disabled": {
            backgroundColor: "#f0f0f0",
            borderColor: "#ccc",
            color: "#aaa",
          },
          "&:focus": {
            outline: "none",
          },
          "&:focus-visible": {
            outline: "2px solid var(--Neutrals-Black-Text)",
            outlineOffset: "2px",
          },
          ...(open && {
            border: "2px solid var(--Neutrals-Black-Text)",
          }),
        }}
      >
        <Box
          sx={{
            width: "100%",
            fontFamily: "var(--font-body), system-ui, sans-serif",
            fontSize: "14px",
            textAlign: "left",
            letterSpacing: "0.01em",
            color: selectedLabel
              ? "var(--Neutrals-Black-Text)"
              : "var(--Neutrals-Medium-Outline)",
          }}
        >
          {selectedLabel || "Select an option"}
        </Box>
        <KeyboardArrowDownIcon
          sx={{
            transition: "transform 0.3s ease-in-out",
            transform: open ? "rotate(180deg)" : "rotate(0)",
            color: "var(--Neutrals-Medium-Outline)",
          }}
        />
      </Button>

      {/* Menu with regular list items */}
      <Menu
        id={`${id}-menu`}
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        disableScrollLock
        MenuListProps={{
          role: "listbox",
          "aria-labelledby": id,
          sx: {
            padding: "0",
            width: "100%",
            outline: "none",
            maxHeight: "300px",
            overflowY: "auto",
            "&::-webkit-scrollbar": {
              width: "10px",
              backgroundColor: "#f5f5f5",
              borderRadius: "8px",
            },
            "&::-webkit-scrollbar-thumb": {
              backgroundColor: "var(--Green-Hover)",
              borderRadius: "8px",
            },
            "&::-webkit-scrollbar-thumb:hover": {
              backgroundColor: "var(--Green-Perfect)",
            },

            scrollbarColor: "var(--Green-Hover) #f5f5f5",
          },
        }}
        PaperProps={{
          sx: {
            borderRadius: "12px",
            border: "1px solid var(--Neutrals-Light-Outline)",
            boxShadow: "0px 4px 8px rgba(0, 0, 0, 0.1)",
            marginTop: "4px",
            width: dropdownRef.current?.clientWidth,
          },
        }}
      >
        {sortedOptions.map((option) => (
          <MenuItem
            key={option.value}
            onClick={() => handleSelect(option.value)}
            sx={{
              borderRadius: "8px",
              border: "1px solid #FFF",
              padding: "10px 16px",
              fontFamily: "var(--font-body), system-ui, sans-serif",
              letterSpacing: "0.01em",
              fontWeight: "500",
              fontSize: "14px",
              color: "var(--Neutrals-Medium-Outline)",
              transition: "all 0.2s ease-in-out",
              "&:hover": {
                backgroundColor: "var(--Beige-Very-Bright)",
                color: "var(--Neutrals-Black-Text)",
                border: "1px solid var(--Neutrals-Light-Outline)",
                fontWeight: "600",
              },
              // Add this new style for focus-visible
              "&:focus-visible": {
                backgroundColor: "var(--Beige-Very-Bright)",
                color: "var(--Neutrals-Black-Text)",
                border: "1px solid var(--Neutrals-Light-Outline)",
                outline: "none",
                fontWeight: "600",
              },
            }}
          >
            {option.label}
          </MenuItem>
        ))}
      </Menu>

      {errorMsg && (
        <Box
          sx={{
            textAlign: "left",
            minHeight: "16px",
            mt: "2px",
            fontSize: "0.875rem",
            color: "#ef4444",
            fontFamily: "var(--font-body), system-ui, sans-serif",
            letterSpacing: "0.01em",
          }}
        >
          {errorMsg}
        </Box>
      )}
    </Box>
  );
}
