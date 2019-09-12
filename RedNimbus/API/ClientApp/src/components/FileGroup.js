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
                        return <File deletingFile={this.props.deletingFile} key={index} name={value} path={this.props.path}/>
                    })
                }
            </div>
        );
    }
}
export default FileGroup;

