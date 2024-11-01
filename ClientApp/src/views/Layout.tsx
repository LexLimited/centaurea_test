import { Divider, Menu, MenuItem, MenuList, MenuPopover, MenuTrigger, makeStyles, tokens } from "@fluentui/react-components";
import { Link, Outlet, useNavigate } from "react-router-dom";
import { I18nLocales, useI18n } from "@/components/I18nContext";
import { Translate24Filled } from "@fluentui/react-icons";
import { useContext } from "react";
import { AuthContext } from "@/context/AuthContext";
import { Button, Typography } from "@mui/material";

const useStyles = makeStyles({
    Root: {
        height: "100vh",
        overflowY: "auto",
        backgroundColor: tokens.colorNeutralBackground3,
        "--header-height": "60px",
        "::before": {
            content: "",
            position: "absolute",
            top: "-200px",
            left: "-200px",
            width: "400px",
            height: "400px",
            backgroundColor: "#c989e8",
            opacity: "0.5",
            filter: "blur(150px)"
        }
    },
    Header: {
        height: "var(--header-height)",
        backgroundColor: tokens.colorNeutralBackground1
    },
    Body: {
        backgroundColor: tokens.colorNeutralBackground2
    },

    LocalePicker: {
        backgroundColor: tokens.colorBrandBackgroundInverted,
        ":hover": {
            backgroundColor: tokens.colorBrandBackgroundInvertedHover
        },
        ":hover:active": {
            backgroundColor: tokens.colorBrandBackgroundInvertedSelected
        }
    }
});

const NavigationList = () => {
    return (
        <div>
            <Link style={{ padding: 10 }} to="/auth">Login</Link>
            <Link style={{ padding: 10 }} to="/datagridcreate">Create</Link>
            <Link style={{ padding: 10 }} to="/datagridlist">Grids</Link>
        </div>
    );
};

export const Layout = () => {
    const styles = useStyles();

    const { authStatus, logOut } = useContext(AuthContext);

    const navigate = useNavigate();

    return (
        <div className={`${styles.Root} col`}>
            <div className={`${styles.Header} row align-center-1 px-5`}>
                <div
                    style={{ marginRight: 36, color: 'blue', cursor: 'pointer' }}
                    onClick={() => navigate('/')}
                >
                    <Typography
                        variant="h5"
                        gutterBottom
                        title={JSON.stringify(authStatus, null, 2)}
                    >
                        {authStatus.username || "Unauthenticated"}
                    </Typography>
                </div>
                <NavigationList />
                <div className="grow"></div>
                {/* <Button
                    variant="contained"
                    color="primary"
                    fullWidth
                    size="large"
                    style={{ width: 85, height: 45 }}
                    onClick={async () => {
                        await logOut(() => navigate('/'));
                    }}
                >
                        Logout
                </Button> */}
                <Divider className="no-grow mx-1" vertical />
            </div>
            <div className={`${styles.Body} grow m-3`}>
                <Outlet />
            </div>
        </div>
    );
};

const LocalePicker = () => {
    const { setI18n } = useI18n();
    const styles = useStyles();

    return (
        <Menu>
            <MenuTrigger>
                <Button icon={<Translate24Filled />} appearance="transparent" className={styles.LocalePicker} />
            </MenuTrigger>
            <MenuPopover>
                <MenuList>
                    {I18nLocales.map((locale) => (
                        <MenuItem key={locale} onClick={() => setI18n(locale)}>
                            {locale}
                        </MenuItem>
                    ))}
                </MenuList>
            </MenuPopover>
        </Menu>
    );
};
