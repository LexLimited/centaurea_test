import { createContext, useContext } from 'react';
import type zhCNfrom from '@/i18n/zh-CN.json';
import { template } from "lodash-es";

type MessageId = keyof typeof zhCNfrom;


class I18nMessage {
  private dict;

  private m = new Map()

  constructor(message: Record<string, string>) {
    this.dict = message;
  }

  /**
   * @example
   * input: ('hello ${ user }!', { user: 'world' })
   * output: 'hello world!'
   */
  get t() {
    return (s: MessageId, data?: Record<string, any>): string => {
      let formatter = this.dict[s]

      if (formatter) {
        if (data) {
          if (this.m.has(formatter)) {
            return this.m.get(formatter)(data)
          } else {
            const complied = template(formatter)
            this.m.set(formatter, complied)
            return complied(data)
          }
        }
        return formatter
      }

      return s
    };
  }
}

export const I18nContext = createContext({ message: new I18nMessage({}), setI18n: (i18n: I18nLocale) => {} });

export const useI18n = () => {
  const { message, setI18n } = useContext(I18nContext);

  return { t: message.t, setI18n };
}

export const makeMessage = (m: Record<string, string>) => new I18nMessage(m);

export const I18nLocales = ["zh-CN", "en-US"] as const;

export type I18nLocale = typeof I18nLocales[number];
