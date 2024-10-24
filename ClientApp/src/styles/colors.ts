import { makeStyles, tokens } from "@fluentui/react-components";

export const useColorStyles = makeStyles({
  bgDanger: {
    backgroundColor: tokens.colorPaletteRedBackground3,
    ":hover": {
      backgroundColor: tokens.colorPaletteRedBackground3,
    },
  },
  bgPrimary: {
    backgroundColor: tokens.colorBrandForegroundLink,
    ":hover": {
      backgroundColor: tokens.colorBrandBackgroundHover,
    },
  },
  fgDanger: {
    color: tokens.colorPaletteRedBackground3,
    ":hover": {
      color: tokens.colorPaletteRedBackground3,
    },
  },
  fgPrimary: {
    color: tokens.colorBrandForeground1,
    ":hover": {
      color: tokens.colorBrandForeground2Hover,
    },
  },
});
