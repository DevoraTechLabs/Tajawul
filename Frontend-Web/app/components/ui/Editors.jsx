"use client";

import React, { useState } from "react";
import Avatar from "@mui/material/Avatar";
import AvatarGroup from "@mui/material/AvatarGroup";
import Modal from "@mui/material/Modal";
import Divider from "@mui/material/Divider";
import styles from "./Editors.module.css";

const Editors = ({ editors }) => {
  const [isPopupOpen, setIsPopupOpen] = useState(false);

  const handleEditorsClick = () => {
    setIsPopupOpen(true);
  };

  const handleClose = () => {
    setIsPopupOpen(false);
  };

  return (
    <div>
      <div className={styles.container} onClick={handleEditorsClick}>
        <span className={styles.important}>All Contributes:</span>
        <div className={styles.editorsClickable}>
          <AvatarGroup max={5}>
            {editors.map((editor) => (
              <Avatar key={editor.id} alt={editor.name} src={editor.url} />
            ))}
          </AvatarGroup>
        </div>
      </div>
      <Modal
        open={isPopupOpen}
        onClose={handleClose}
        className={styles.popupOverlay}
      >
        <div className={styles.popupContent}>
          <h2>Editors</h2>
          <Divider
            sx={{
              height: "1px",
              width: "100%",
              bgcolor: "var(--Neutrals-Light-Outline)",
            }}
          />
          <div className={styles.editorsContainer}>
            {editors.map((editor) => (
              <div key={editor.id} className={styles.editorItem}>
                <Avatar alt={editor.name} src={editor.url} />
                <span>{editor.name}</span>
              </div>
            ))}
          </div>
        </div>
      </Modal>
    </div>
  );
};

export default Editors;
