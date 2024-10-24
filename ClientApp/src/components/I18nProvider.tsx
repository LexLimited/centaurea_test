import { PropsWithChildren, useEffect, useState } from "react";
import { I18nContext, I18nLocale, makeMessage } from "./I18nContext";

export const I18nProvider = ({ children }: PropsWithChildren) => {
    const [message, setMessage] = useState(makeMessage({}));

    const setI18n = (i18n: I18nLocale) => {
        import(`@/i18n/${i18n}.json`)
            .then((res) => setMessage(makeMessage(res.default)))
            .then(() => localStorage.setItem("i18n", i18n));
    };

    useEffect(() => {
        const i18n = (localStorage.getItem("i18n") ?? "zh-CN") as I18nLocale;
        setI18n(i18n);
    }, []);

    return <I18nContext.Provider value={{ message, setI18n }}>{children}</I18nContext.Provider>;
};
