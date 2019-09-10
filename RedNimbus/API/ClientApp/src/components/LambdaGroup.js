import React from 'react';
import Lambda from './Lambda';

class LambdaGroup extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="card-group">
                {
                    this.props.content.map((value, index) => {
                        return <Lambda key={index} name={value} />
                    })
                }
            </div>
        );
    }
}
export default LambdaGroup;

