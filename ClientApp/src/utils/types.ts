type IsEmptyObjectOrNonRecord<T> =
    [keyof T] extends [never]
        ? true
        : T extends Record<string, unknown>
            ? false
            : true

type KeyPath<T> = 
    keyof T extends infer K extends keyof T
        ? K extends K
            ? IsEmptyObjectOrNonRecord<T[K]> extends true
                ? [K]
                : [K, ...KeyPath<T[K]>]
            : never
        : never;

type PartialTuple<T> = 
    T extends [infer I, ...infer R] 
        ? [I] | [I, ...PartialTuple<R>] 
        : never;

type PartialKeyPath<T> = PartialTuple<KeyPath<T>>

type DeepRequired<T> =
    T extends Record<any, unknown> 
        ? { [K in keyof T]-?: DeepRequired<T[K]> }  
        : NonNullable<T>

type DeepOptional<T, P extends PartialKeyPath<T>> = 
    P extends [infer I extends keyof T, ...infer R]
        ? R extends PartialKeyPath<T[I]>
            ? Prettier<{ [k in Exclude<keyof T, I>]: T[k] } & { [k in I]: DeepOptional<T[k], R> }>
            : Prettier<{ [k in Exclude<keyof T, I>]: T[k] } & { [k in I]?: T[k] }>
        : never;

type Prettier<T> =
    T extends T
        ? { [k in keyof T]: T[k] }
        : never
