import React from 'react';
import File from './File';

class FileGroup extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="card-group">
                {
                    this.props.content.map((value, index) => {
                        return <File key={index} name={value}/>
                    })
                }
            </div>
        );
    }
}
export default FileGroup;

