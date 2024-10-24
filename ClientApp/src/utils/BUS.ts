class Bus {
  private m: Registry = {};

  sub<T>(event: BusEvent<T>, handler: EventHandler<BusEvent<T>>) {
    if (!this.m[event]) this.m[event] = [];
    if (!this.m[event].includes(handler)) this.m[event].push(handler);
  }

  unsub<T>(event: BusEvent<T>, handler: EventHandler<BusEvent<T>>) {
    if (!this.m[event]) return;
    const idx = this.m[event].findIndex(it => it === handler);
    if (idx >= 0) this.m[event].splice(idx, 1);
  }

  pub<T>(event: BusEvent<T>, payload: T) {
    if (this.m[event]) {
      this.m[event].forEach(cb => cb(payload));
    }
  }
}

export const BUS = new Bus();

export const makeBusEvent = <T>(name: string) => Symbol(name) as BusEvent<T>;

type BusEvent<T> = symbol & T;

type EventHandler<E extends BusEvent<unknown>> = E extends BusEvent<infer I> ? (payload: I) => any : never;

type Registry<T = unknown> = Record<BusEvent<T>, EventHandler<BusEvent<T>>[]>;
