import { useState } from "react";
import { useMsal, useIsAuthenticated } from "@azure/msal-react";
import IconButton from "@mui/material/IconButton";
import AccountCircle from "@mui/icons-material/AccountCircle";
import MenuItem from "@mui/material/MenuItem";
import Menu from "@mui/material/Menu";
import { AccountPicker } from "./AccountPicker";
import { b2cPolicies } from "../authConfig";
import { InteractionStatus } from "@azure/msal-browser";

export const SignOutButton = () => {
  const { instance } = useMsal();
  const [accountSelectorOpen, setOpen] = useState(false);
  const isAuthenticated = useIsAuthenticated();
  const { inProgress } = useMsal();

  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);

  const handleLogout = (logoutType) => {
    setAnchorEl(null);

    if (logoutType === "popup") {
      instance.logoutPopup();
    } else if (logoutType === "redirect") {
      instance.logoutRedirect();
    }
  };

  const handleAccountSelection = () => {
    setAnchorEl(null);
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };

  const handleProfile = () => {
    if (isAuthenticated && inProgress === InteractionStatus.None) {
      instance.acquireTokenRedirect(b2cPolicies.authorities.editProfile);
    }
  };

  return (
    <div>
      <IconButton
        onClick={(event) => setAnchorEl(event.currentTarget)}
        color="inherit"
      >
        <AccountCircle />
      </IconButton>
      <Menu
        id="menu-appbar"
        anchorEl={anchorEl}
        anchorOrigin={{
          vertical: "top",
          horizontal: "right",
        }}
        keepMounted
        transformOrigin={{
          vertical: "top",
          horizontal: "right",
        }}
        open={open}
        onClose={() => setAnchorEl(null)}
      >
        <MenuItem onClick={() => handleAccountSelection()} key="switchAccount">
          Switch Account
        </MenuItem>
        <MenuItem onClick={() => handleLogout("popup")} key="logoutPopup">
          Logout using Popup
        </MenuItem>
        <MenuItem onClick={() => handleLogout("redirect")} key="logoutRedirect">
          Logout using Redirect
        </MenuItem>
        <MenuItem onClick={() => handleProfile()} key="profile">
          Profile
        </MenuItem>
      </Menu>
      <AccountPicker open={accountSelectorOpen} onClose={handleClose} />
    </div>
  );
};
