import { Alert } from '@fluentui/react-components/unstable';
import { PropsWithChildren, useState, useEffect, useRef } from 'react';
import { uniqueId } from "lodash-es";
import { GlobalNotificationContext, NotifyMessage } from './GlobalNotifierContext';

export const GlobalNotificationProvider = ({ children }: PropsWithChildren<{}>) => {
  const [notifications, setNotifications] = useState<NotifyMessage[]>([]);
  const schedule = useRef<string[]>([]);

  const addNotification = (notification: NotifyMessage) =>
    setNotifications([...notifications, { ...notification, _id: uniqueId() }]);

  const removeNotification = (_id: string) => setNotifications(notifications.filter(n => n._id !== _id));

  useEffect(() => {
    if (!notifications.length) return;
    const notification = notifications.at(-1)!;

    if (notification.keep || schedule.current.includes(notification._id!)) return;
    else schedule.current.push(notification._id!);

    setTimeout(() => {
      schedule.current = schedule.current.filter(n => n !== notification._id);
      setNotifications(notifications.filter(n => schedule.current.includes(n._id!)));
    }, 2000);
  }, [notifications, schedule]);

  return (
    <GlobalNotificationContext.Provider value={{ notifications, addNotification, removeNotification }}>
      {children}
      <div id="notifications" className="col gap-2 p-3" style={{ position: 'fixed', bottom: 0, right: 0 }}>
        {notifications.map(notification => (
          <Alert intent={notification.intent} key={notification._id} action={notification.action}>
            {notification.message}
          </Alert>
        ))}
      </div>
    </GlobalNotificationContext.Provider>
  );
};
