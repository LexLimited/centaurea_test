import { createRoot } from 'react-dom/client';
import { FluentProvider, teamsLightTheme } from '@fluentui/react-components';
import { RouterProvider } from "react-router-dom";
import { GlobalDialogProvider } from "./components/GlobalDialogProvider";
import { GlobalNotificationProvider } from "./components/GlobalNotifierProvider";
import { I18nProvider } from './components/I18nProvider';
import router from './router';
import '@/styles/global.css';

createRoot(document.getElementById('root')!).render(
  <FluentProvider theme={teamsLightTheme}>
    <GlobalDialogProvider>
      <GlobalNotificationProvider>
        <I18nProvider>
          <RouterProvider router={router} />
        </I18nProvider>
      </GlobalNotificationProvider>
    </GlobalDialogProvider>
  </FluentProvider>
);
