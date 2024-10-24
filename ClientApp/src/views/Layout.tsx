import { Button, Divider, Menu, MenuItem, MenuList, MenuPopover, MenuTrigger, makeStyles, tokens } from "@fluentui/react-components";
import { Outlet } from "react-router-dom";
import { I18nLocales, useI18n } from "@/components/I18nContext";
import { Translate24Filled } from "@fluentui/react-icons";

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

export const Layout = () => {
    const styles = useStyles();

    return (
        <div className={`${styles.Root} col`}>
            <div className={`${styles.Header} row align-center-1 px-5`}>
                <div className="grow"></div>
                <LocalePicker />
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
