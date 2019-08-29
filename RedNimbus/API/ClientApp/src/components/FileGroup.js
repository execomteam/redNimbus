import React from 'react';
import File from './File';

class FileGroup extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div class="card-group">
                {
                    this.props.content.map((value, index) => {
                        return <File name={value}/>
                    })
                }
            </div>
        );
    }
}
export default FileGroup;

