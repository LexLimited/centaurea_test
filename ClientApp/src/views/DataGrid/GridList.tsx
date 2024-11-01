import { CentaureaApi } from "@/api/CentaureaApi";
import { CButton } from "@/components/CButton";
import { Models } from "@/Models/DataGrid";
import React from "react";
import { useNavigate } from "react-router-dom";

function GridContainer({ children, style }: any) {
    return (
        <div style={{ height: 400, width: '100%', ...style }}>
            {children}
        </div>
    )
}

export function GridList({
    onSelect,
}: {
    onSelect?: (gridId: number) => void,
}) {
    const [gridDescriptors, setGridDescriptors] = React.useState<Models.Dto.DataGridDescriptor[]>([]);

    const [selectedId, setSelectedId] = React.useState<number | undefined>();

    const navigate = useNavigate();

    React.useEffect(() => {
        CentaureaApi.getGridDescriptors()
            .then(res => setGridDescriptors(res.data));
    }, []);

    if (!gridDescriptors.length) {
        return (
            <GridContainer>No grids found</GridContainer>
        )
    }

    const options = gridDescriptors.map(descriptor => (
        <CButton
            key={descriptor.id}
            disabled={selectedId == descriptor.id}
            onClick={() => {
                navigate(`/datagridedit?gridId=${descriptor.id}`);
            }}
            style={{ width: 175 }}
        >
            {JSON.stringify(descriptor.name)}
        </CButton>
    ));

    return (
        <GridContainer>
            <div style={{ padding: 10, width: 100, display: 'flex', flexDirection: 'column' }}>{options}</div>
        </GridContainer>
    )
}
