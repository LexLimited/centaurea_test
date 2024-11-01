import { Button } from "@mui/material"

export const CButton = ({children, style, ...props}: any) => {
    return <Button style={{padding: 10, margin: 5, ...style}} {...props}>{children}</Button>
}