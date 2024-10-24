import { AlertProps } from '@fluentui/react-components/unstable';
import { createContext, useContext } from 'react';

export type NotifyMessage = {
  intent: AlertProps['intent'];
  message: string;
  action?: AlertProps['action'];
  keep?: boolean;
  _id?: string;
};

export const GlobalNotificationContext = createContext({
  notifications: [],
  addNotification: () => {},
  removeNotification: () => {},
} as {
  notifications: NotifyMessage[];
  addNotification: (notification: NotifyMessage) => void;
  removeNotification: (_id: string) => void;
});

export const useGlobalNotification = () => useContext(GlobalNotificationContext);
