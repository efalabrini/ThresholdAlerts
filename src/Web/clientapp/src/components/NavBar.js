import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Link from "@mui/material/Link";
import Typography from "@mui/material/Typography";
import WelcomeName from "./WelcomeName";
import SignInSignOutButton from "./SignInSignOutButton";

const NavBar = () => {
    return (
        <div sx={{ flexGrow: 1}}>
            <AppBar position="static">
            <Toolbar>
                <Typography sx={{ flexGrow: 1 }}>
                <p style={{ textAlign: 'left', fontSize: '20px' }}>ThresholdAlert</p>
                </Typography>
                <WelcomeName />
                <SignInSignOutButton />
            </Toolbar>
            </AppBar>
        </div>
    );
};

export default NavBar;