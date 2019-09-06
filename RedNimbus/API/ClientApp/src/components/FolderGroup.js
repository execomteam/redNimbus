import React from 'react';
import Folder from './Folder';

class FolderGroup extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return(
            <div className="card-group">
                {
                    this.props.content.map((value, index) => {
                        return <Folder deletingBucket = {this.props.deletingBucket} key={index} name={value}/>
                    })
                }
            </div>
        );
    }
}
export default FolderGroup;

