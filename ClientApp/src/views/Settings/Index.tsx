import { Tab, TabList, Text } from "@fluentui/react-components";
import { useI18n } from "@/components/I18nContext";

export const Index = () => {
    const { t } = useI18n();

    return (
        <div className="col">
            <Text weight="bold" size={700}>
                {t("Upper.Settings")}
            </Text>

            <TabList>
                <Tab value={"SystemSettings"}>{t("Upper.SystemSettings")}</Tab>
            </TabList>
        </div>
    );
};
